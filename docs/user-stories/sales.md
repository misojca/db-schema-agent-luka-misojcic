# Sales Domain — User Stories

---

## SALES-STORY-001: Place and Track Sales Orders

**Architecture Reference**: Section 5.5 — Building Block View (Sales Schema)
**Priority**: CORE
**Pareto**: 20% core (delivers 80% value)

AS A customer
I WANT to place a sales order for one or more products and track its fulfilment
status
SO THAT I can receive the goods I need and have a clear record of every
transaction with AdventureWorks

### SCENARIO 1: Place a new sales order
**Scenario ID**: SALES-STORY-001-S1

**GIVEN**
* A customer with `customerid = 42` exists in `sales.customer`
* A valid ship method with `shipmethodid = 1` exists in `purchasing.shipmethod`
* A valid bill-to address with `addressid = 10` and a ship-to address
  with `addressid = 11` exist in `person.address`
* Products `776` and `777` exist in `production.product` with active
  sell dates covering today
* A default special offer with `specialofferid = 1` exists in
  `sales.specialoffer`

**WHEN**
* A customer submits a new sales order with two line items:
  - product `776`, quantity `2`, unit price `782.99`
  - product `777`, quantity `1`, unit price `602.50`

**THEN**
* A new row is inserted into `sales.salesorderheader` with
  `status = 1` (In Process), `revisionnumber = 1`,
  `orderdate` set to the current UTC timestamp,
  `onlineorderflag = true`, and `customerid = 42`
* Two new rows are inserted into `sales.salesorderdetail` linked to
  the new header, each with the correct `productid`, `orderqty`,
  `unitprice`, and `unitpricediscount = 0`
* `subtotal` on the header equals the sum of `(unitprice * orderqty)`
  across all line items
* The response returns HTTP 201 with the full order including the
  generated `salesorderid`

### SCENARIO 2: Retrieve a sales order with all line items
**Scenario ID**: SALES-STORY-001-S2

**GIVEN**
* A sales order with `salesorderid = 100` exists in
  `sales.salesorderheader` with three detail lines

**WHEN**
* A GET request is made for sales order `100`

**THEN**
* The response returns HTTP 200
* The payload includes the header fields (`salesorderid`, `status`,
  `customerid`, `orderdate`, `duedate`, `shipdate`, `subtotal`,
  `taxamt`, `freight`, `totaldue`)
* A `lineItems` array contains all three detail rows, each including
  `salesorderdetailid`, `productid`, `orderqty`, `unitprice`,
  `unitpricediscount`, and `carriertrackingnumber`

### SCENARIO 3: Retrieve all orders for a customer
**Scenario ID**: SALES-STORY-001-S3

**GIVEN**
* Customer `42` has three sales orders in `sales.salesorderheader`

**WHEN**
* A GET request is made for all orders belonging to customer `42`

**THEN**
* The response returns HTTP 200
* The payload contains exactly three order summaries, each including
  `salesorderid`, `orderdate`, `status`, and `totaldue`
* Orders are returned in descending `orderdate` order (most recent first)

### SCENARIO 4: Attempt to place an order referencing a non-existent customer
**Scenario ID**: SALES-STORY-001-S4

**GIVEN**
* No customer with `customerid = 9999` exists in `sales.customer`

**WHEN**
* A POST request is submitted with `customerId = 9999`

**THEN**
* The API returns HTTP 422 Unprocessable Entity
* The response body contains a validation error:
  `"Customer 9999 does not exist"`
* No rows are inserted into `salesorderheader` or `salesorderdetail`

### SCENARIO 5: Update the ship date on a sales order
**Scenario ID**: SALES-STORY-001-S5

**GIVEN**
* A sales order with `salesorderid = 100` has `shipdate = null`
  and `status = 1` (In Process)

**WHEN**
* A PATCH request sets `shipDate = "2026-09-20T00:00:00Z"`

**THEN**
* `shipdate` is updated in `sales.salesorderheader`
* `revisionnumber` is incremented by 1
* `modifieddate` is updated to the current UTC timestamp
* The response returns HTTP 200 with the updated header

### SCENARIO 6: Attempt to ship an order that is already cancelled
**Scenario ID**: SALES-STORY-001-S6

**GIVEN**
* A sales order with `salesorderid = 101` has `status = 6` (Cancelled)

**WHEN**
* A PATCH request attempts to set `shipDate` on order `101`

**THEN**
* The API returns HTTP 422 Unprocessable Entity
* The response body contains:
  `"Cannot update a cancelled sales order"`
* No changes are persisted to `sales.salesorderheader`

---

### SALES-BE-001.1: Sales Order Header API
**Architecture Reference**: Section 5.5 — Building Block View (Sales Schema)

AS A developer
I WANT REST endpoints to create, retrieve, and update sales order headers
SO THAT customers and sales representatives can manage orders over HTTP

#### SCENARIO 1: POST /sales/orders creates a new sales order
**Scenario ID**: SALES-BE-001.1-S1

**GIVEN**
* Customer `42`, ship method `1`, bill-to address `10`, ship-to
  address `11`, products `776` and `777`, and special offer `1`
  all exist in the database

**WHEN**
* `POST /sales/orders` is called with:
  ```json
  {
    "customerId": 42,
    "shipMethodId": 1,
    "billToAddressId": 10,
    "shipToAddressId": 11,
    "onlineOrderFlag": true,
    "lineItems": [
      { "productId": 776, "orderQty": 2, "unitPrice": 782.99, "specialOfferId": 1 },
      { "productId": 777, "orderQty": 1, "unitPrice": 602.50, "specialOfferId": 1 }
    ]
  }
  ```

**THEN**
* HTTP 201 is returned
* Response body contains the new `salesOrderId`, `status: 1`,
  `revisionNumber: 1`, computed `subtotal`, and both line items
* `Location` header is set to `/sales/orders/{salesOrderId}`

#### SCENARIO 2: POST /sales/orders returns 422 for a non-existent customer
**Scenario ID**: SALES-BE-001.1-S2

**GIVEN**
* No customer with `customerId = 9999` exists in `sales.customer`

**WHEN**
* `POST /sales/orders` is called with `"customerId": 9999`

**THEN**
* HTTP 422 is returned
* Response body contains:
  ```json
  { "errors": { "customerId": ["Customer 9999 does not exist"] } }
  ```
* No rows are inserted

#### SCENARIO 3: POST /sales/orders returns 422 for a discontinued product
**Scenario ID**: SALES-BE-001.1-S3

**GIVEN**
* Product `999` has a `discontinueddate` in the past

**WHEN**
* `POST /sales/orders` is called with a line item referencing `productId: 999`

**THEN**
* HTTP 422 is returned
* Response body contains:
  ```json
  { "errors": { "lineItems[0].productId": ["Product 999 is discontinued and cannot be ordered"] } }
  ```

#### SCENARIO 4: GET /sales/orders/{id} returns the full order
**Scenario ID**: SALES-BE-001.1-S4

**GIVEN**
* Sales order `100` exists with three line items

**WHEN**
* `GET /sales/orders/100` is called

**THEN**
* HTTP 200 is returned
* Response body includes `salesOrderId: 100`, customer details,
  header totals (`subtotal`, `taxAmt`, `freight`, `totalDue`),
  and a `lineItems` array with all three detail rows

#### SCENARIO 5: GET /sales/orders/{id} returns 404 when not found
**Scenario ID**: SALES-BE-001.1-S5

**GIVEN**
* No sales order with `salesorderid = 9999` exists

**WHEN**
* `GET /sales/orders/9999` is called

**THEN**
* HTTP 404 is returned
* Response body contains a problem details object with
  `title: "Sales Order Not Found"`

#### SCENARIO 6: GET /sales/orders?customerId={id} returns all orders for a customer
**Scenario ID**: SALES-BE-001.1-S6

**GIVEN**
* Customer `42` has three sales orders

**WHEN**
* `GET /sales/orders?customerId=42` is called

**THEN**
* HTTP 200 is returned
* Response body is an array of three order summaries sorted by
  `orderDate` descending
* Each summary includes `salesOrderId`, `orderDate`, `status`,
  and `totalDue`

#### SCENARIO 7: PATCH /sales/orders/{id} updates the ship date
**Scenario ID**: SALES-BE-001.1-S7

**GIVEN**
* Sales order `100` has `status = 1` and `shipDate = null`

**WHEN**
* `PATCH /sales/orders/100` is called with:
  ```json
  { "shipDate": "2026-09-20T00:00:00Z" }
  ```

**THEN**
* HTTP 200 is returned
* Response body shows the updated `shipDate` and incremented
  `revisionNumber`
* `modifiedDate` reflects the current UTC timestamp

#### SCENARIO 8: PATCH /sales/orders/{id} returns 422 when order is cancelled
**Scenario ID**: SALES-BE-001.1-S8

**GIVEN**
* Sales order `101` has `status = 6` (Cancelled)

**WHEN**
* `PATCH /sales/orders/101` is called with any update payload

**THEN**
* HTTP 422 is returned
* Response body contains:
  ```json
  { "errors": { "": ["Cannot update a cancelled sales order"] } }
  ```

---

### SALES-BE-001.2: Sales Order Detail API
**Architecture Reference**: Section 5.5 — Building Block View (Sales Schema)

AS A developer
I WANT REST endpoints to manage individual line items on a sales order
SO THAT order contents can be reviewed and updated independently of the header

#### SCENARIO 1: GET /sales/orders/{id}/lines returns all line items
**Scenario ID**: SALES-BE-001.2-S1

**GIVEN**
* Sales order `100` has three detail lines in `sales.salesorderdetail`

**WHEN**
* `GET /sales/orders/100/lines` is called

**THEN**
* HTTP 200 is returned
* Response body is an array of three items, each containing
  `salesOrderDetailId`, `productId`, `orderQty`, `unitPrice`,
  `unitPriceDiscount`, and `carrierTrackingNumber`

#### SCENARIO 2: PATCH /sales/orders/{id}/lines/{lineId} updates carrier tracking number
**Scenario ID**: SALES-BE-001.2-S2

**GIVEN**
* Sales order detail `5` belongs to order `100`
* Current `carrierTrackingNumber` is `null`

**WHEN**
* `PATCH /sales/orders/100/lines/5` is called with:
  ```json
  { "carrierTrackingNumber": "1Z999AA10123456784" }
  ```

**THEN**
* HTTP 200 is returned
* Response body shows `carrierTrackingNumber: "1Z999AA10123456784"`
* `modifiedDate` is updated to the current UTC timestamp

#### SCENARIO 3: PATCH /sales/orders/{id}/lines/{lineId} returns 404 when line does not belong to order
**Scenario ID**: SALES-BE-001.2-S3

**GIVEN**
* Sales order `100` exists, but detail line `999` belongs to a
  different order

**WHEN**
* `PATCH /sales/orders/100/lines/999` is called

**THEN**
* HTTP 404 is returned
* Response body contains a problem details object with
  `title: "Sales Order Detail Not Found"`

---

### SALES-BE-001.3: SalesOrderHeader and SalesOrderDetail EF Core Entities
**Architecture Reference**: Section 9 — Architecture Decisions (ADR-001)

AS A developer
I WANT `SalesOrderHeader` and `SalesOrderDetail` entities with
`IEntityTypeConfiguration<T>` Fluent API configuration classes
SO THAT EF Core maps the `sales` schema tables correctly without
polluting the domain entities with EF Core attributes

#### SCENARIO 1: Entities map to correct schema and table names
**Scenario ID**: SALES-BE-001.3-S1

**GIVEN**
* `SalesOrderHeaderConfiguration` and `SalesOrderDetailConfiguration`
  are registered in `AdventureWorksDbContext`

**WHEN**
* `dotnet ef migrations add` is run targeting the `sales` schema

**THEN**
* The generated migration targets schema `sales`
* Table names are `salesorderheader` and `salesorderdetail`
* `salesorderid` is the primary key for the header (identity)
* The composite key `(salesorderid, salesorderdetailid)` is configured
  on `salesorderdetail`
* No EF Core data annotations appear on the domain entity classes

#### SCENARIO 2: Navigation property from header to detail lines is configured
**Scenario ID**: SALES-BE-001.3-S2

**GIVEN**
* `SalesOrderHeader` has a collection navigation property `LineItems`
  of type `ICollection<SalesOrderDetail>`
* The Fluent API configures a one-to-many relationship

**WHEN**
* A `SalesOrderHeader` is loaded with `.Include(h => h.LineItems)`

**THEN**
* All associated `SalesOrderDetail` rows are populated in `LineItems`
* No additional queries are issued beyond the eager-loaded join

#### SCENARIO 3: Computed totaldue column is correctly mapped
**Scenario ID**: SALES-BE-001.3-S3

**GIVEN**
* `totaldue` in `sales.salesorderheader` is a nullable `numeric` column
  computed as `subtotal + taxamt + freight` at the application layer

**WHEN**
* The `SalesOrderHeaderConfiguration` maps `totaldue` as a regular
  column with no generated-value convention

**THEN**
* Inserting a `SalesOrderHeader` with explicit `subtotal`, `taxamt`,
  `freight`, and `totaldue` persists all four values correctly
* No database-side computed column expression is applied by the migration

---

## Pareto Note

SALES-STORY-001 is classified **CORE** because:
- Sales order management is the primary revenue-generating flow in the
  Sales bounded context (Section 1.2) and the most visible end-user feature
- `salesorderheader.customerid → sales.customer` is an explicit
  cross-schema FK listed in Section 3.3
- It exercises the full architecture stack: API endpoints (BE), EF Core
  entity configuration (BE), EF Core migration (INFRA), FK integrity
  (INFRA), and observability (INFRA)
- It is a prerequisite for downstream reporting flows (revenue, territory
  performance) that depend on a populated `salesorderheader` table

---

### SALES-INFRA-001.1: Expose ASP.NET Core Web API endpoint
**Architecture Reference**: Section 3 — Context and Scope

AS A developer
I WANT the sales order endpoints exposed through `AdventureWorks.Api`
SO THAT customers and sales representatives can place and track orders
over HTTP

#### SCENARIO 1: GET endpoint responds successfully
**Scenario ID**: SALES-INFRA-001.1-S1

**GIVEN**
* The solution builds successfully (`dotnet build`)
* `AdventureWorks.Api` is running with a valid database connection

**WHEN**
* `GET /sales/orders/100` is called for an existing sales order

**THEN**
* The endpoint returns HTTP 200
* The response `Content-Type` is `application/json`
* The payload includes the sales order header and its line items

#### SCENARIO 2: POST endpoint creates a sales order end-to-end
**Scenario ID**: SALES-INFRA-001.1-S2

**GIVEN**
* The solution builds successfully
* Customer `42`, ship method `1`, addresses `10` and `11`, products
  `776` and `777`, and special offer `1` all exist in the database

**WHEN**
* `POST /sales/orders` is called with a valid request body

**THEN**
* HTTP 201 is returned
* The `Location` header points to the newly created resource
* The database contains the new `salesorderheader` row and at least one
  `salesorderdetail` row

#### SCENARIO 3: Non-existent route returns 404
**Scenario ID**: SALES-INFRA-001.1-S3

**GIVEN**
* `AdventureWorks.Api` is running

**WHEN**
* `GET /sales/nonexistent` is called

**THEN**
* The API returns HTTP 404 with a standard problem details body

---

### SALES-INFRA-001.2: Apply EF Core migration for sales schema
**Architecture Reference**: Section 5.5 — Building Block View (Sales Schema) · Section 9 — Architecture Decisions (ADR-001)

AS A developer
I WANT the `sales.salesorderheader` and `sales.salesorderdetail` tables
verified and all required entity configurations applied through EF Core
migrations
SO THAT the schema supports sales order persistence with correct types,
constraints, and composite keys

#### SCENARIO 1: Migration applies cleanly
**Scenario ID**: SALES-INFRA-001.2-S1

**GIVEN**
* PostgreSQL instance is running (AdventureWorks, port 5433)
* The migration for `SALES-STORY-001` has been generated

**WHEN**
* `dotnet ef database update` is executed

**THEN**
* Schema `sales` exists
* Table `sales.salesorderheader` contains the following columns with
  the correct types and nullability:

  | Column | Type | Nullable |
  |--------|------|----------|
  | salesorderid | integer (identity) | NOT NULL |
  | revisionnumber | smallint | NOT NULL |
  | orderdate | timestamp without time zone | NOT NULL |
  | duedate | timestamp without time zone | NOT NULL |
  | shipdate | timestamp without time zone | NULL |
  | status | smallint | NOT NULL |
  | onlineorderflag | boolean | NOT NULL |
  | purchaseordernumber | character varying | NULL |
  | accountnumber | character varying | NULL |
  | customerid | integer | NOT NULL |
  | salespersonid | integer | NULL |
  | territoryid | integer | NULL |
  | billtoaddressid | integer | NOT NULL |
  | shiptoaddressid | integer | NOT NULL |
  | shipmethodid | integer | NOT NULL |
  | creditcardid | integer | NULL |
  | creditcardapprovalcode | character varying | NULL |
  | currencyrateid | integer | NULL |
  | subtotal | numeric | NOT NULL |
  | taxamt | numeric | NOT NULL |
  | freight | numeric | NOT NULL |
  | totaldue | numeric | NULL |
  | comment | character varying | NULL |
  | rowguid | uuid | NOT NULL |
  | modifieddate | timestamp without time zone | NOT NULL |

* Table `sales.salesorderdetail` contains the following columns:

  | Column | Type | Nullable |
  |--------|------|----------|
  | salesorderid | integer | NOT NULL |
  | salesorderdetailid | integer | NOT NULL |
  | carriertrackingnumber | character varying | NULL |
  | orderqty | smallint | NOT NULL |
  | productid | integer | NOT NULL |
  | specialofferid | integer | NOT NULL |
  | unitprice | numeric | NOT NULL |
  | unitpricediscount | numeric | NOT NULL |
  | rowguid | uuid | NOT NULL |
  | modifieddate | timestamp without time zone | NOT NULL |

* Primary key on `salesorderheader` is `salesorderid`
* Composite primary key on `salesorderdetail` is
  `(salesorderid, salesorderdetailid)`

#### SCENARIO 2: Round-trip test passes
**Scenario ID**: SALES-INFRA-001.2-S2

**GIVEN**
* The migration has been applied successfully

**WHEN**
* `dotnet ef database update 0` is run (rolls back all migrations)
* `dotnet ef database update` is run again (re-applies all migrations)

**THEN**
* Both operations complete without errors
* Tables `sales.salesorderheader` and `sales.salesorderdetail` exist
  with the correct structure after re-application

---

### SALES-INFRA-001.3: Enforce referential integrity
**Architecture Reference**: Section 1.3 — Quality Goals (Priority 1) · Section 3.3 — Cross-Schema Dependencies

AS A developer
I WANT foreign key constraints on `sales.salesorderheader` and
`sales.salesorderdetail` enforced at the database level
SO THAT sales orders cannot reference non-existent customers, addresses,
or ship methods, and detail lines cannot reference non-existent headers
or products

#### SCENARIO 1: FK constraint rejects a sales order for a non-existent customer
**Scenario ID**: SALES-INFRA-001.3-S1

**GIVEN**
* The migration for `SALES-STORY-001` has been applied
* No customer with `customerid = 9999` exists in `sales.customer`

**WHEN**
* An INSERT is attempted into `sales.salesorderheader` with
  `customerid = 9999`

**THEN**
* The database rejects the operation with a foreign key violation error
  referencing `salesorderheader.customerid → customer.customerid`
* No row is inserted into `sales.salesorderheader`

#### SCENARIO 2: FK constraint rejects a detail line for a non-existent header
**Scenario ID**: SALES-INFRA-001.3-S2

**GIVEN**
* No sales order with `salesorderid = 88888` exists in
  `sales.salesorderheader`

**WHEN**
* An INSERT is attempted into `sales.salesorderdetail` with
  `salesorderid = 88888`

**THEN**
* The database rejects the operation with a foreign key violation error
  referencing `salesorderdetail.salesorderid → salesorderheader.salesorderid`
* No row is inserted into `sales.salesorderdetail`

#### SCENARIO 3: FK constraint rejects a sales order for a non-existent bill-to address
**Scenario ID**: SALES-INFRA-001.3-S3

**GIVEN**
* No address with `addressid = 77777` exists in `person.address`

**WHEN**
* An INSERT is attempted into `sales.salesorderheader` with
  `billtoaddressid = 77777`

**THEN**
* The database rejects the operation with a foreign key violation error
  referencing `salesorderheader.billtoaddressid → address.addressid`
* No row is inserted into `sales.salesorderheader`

#### SCENARIO 4: Deleting a customer with existing sales orders is rejected
**Scenario ID**: SALES-INFRA-001.3-S4

**GIVEN**
* Customer `42` exists in `sales.customer`
* At least one sales order with `customerid = 42` exists in
  `sales.salesorderheader`

**WHEN**
* A DELETE is attempted on `sales.customer` for `customerid = 42`

**THEN**
* The database rejects the DELETE with a foreign key violation error
* The customer row remains in `sales.customer`
* The sales order rows are not affected

---

### SALES-INFRA-001.4: Setup monitoring and observability
**Architecture Reference**: Section 3.2 — External Interfaces

AS A developer
I WANT structured logging on all sales order endpoints and a health check
SO THAT I can monitor service health and diagnose order fulfilment issues
in production

#### SCENARIO 1: Health check is reachable
**Scenario ID**: SALES-INFRA-001.4-S1

**GIVEN**
* `AdventureWorks.Api` is running with a valid database connection

**WHEN**
* `GET /health` is called

**THEN**
* Response is HTTP 200 OK
* Response body contains `{ "status": "Healthy" }`

#### SCENARIO 2: Structured log entry is emitted on sales order creation
**Scenario ID**: SALES-INFRA-001.4-S2

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`
  (e.g., Serilog or Microsoft.Extensions.Logging with JSON output)

**WHEN**
* `POST /sales/orders` is called and a new sales order is created

**THEN**
* A structured log entry at `Information` level is emitted containing:
  * A message template identifying the create operation
    (e.g., `"SalesOrder {SalesOrderId} created for customer {CustomerId}"`)
  * `salesOrderId` of the new order
  * `customerId` and line item count
* No sensitive payment data (e.g., full card numbers) appears in the log

#### SCENARIO 3: Structured log entry is emitted on ship date update
**Scenario ID**: SALES-INFRA-001.4-S3

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`

**WHEN**
* `PATCH /sales/orders/{id}` is called and the ship date is updated

**THEN**
* A structured log entry at `Information` level is emitted containing:
  * `salesOrderId`
  * `newShipDate`
  * `revisionNumber` after the update

#### SCENARIO 4: Structured log entry is emitted at Warning level on validation failure
**Scenario ID**: SALES-INFRA-001.4-S4

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`

**WHEN**
* `POST /sales/orders` is called with an invalid `customerId` and
  returns HTTP 422

**THEN**
* A structured log entry at `Warning` level is emitted containing:
  * The validation error message
  * The offending `customerId`
* The log entry does not include a full stack trace (validation failures
  are expected paths, not unhandled exceptions)

#### SCENARIO 5: Structured log entry is emitted at Warning level on attempt to update a cancelled order
**Scenario ID**: SALES-INFRA-001.4-S5

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`

**WHEN**
* `PATCH /sales/orders/101` is called for a cancelled order and
  returns HTTP 422

**THEN**
* A structured log entry at `Warning` level is emitted containing:
  * `salesOrderId: 101`
  * `currentStatus: 6`
  * The business rule violation message:
    `"Attempted to update cancelled sales order"`
