# Purchasing Domain — User Stories

---

## PUR-STORY-001: Create and Track Purchase Orders

**Architecture Reference**: Section 5.4 — Building Block View (Purchasing Schema)
**Priority**: CORE
**Pareto**: 20% core (delivers 80% value)

AS A purchasing officer
I WANT to create purchase orders against vendors and track their fulfilment status
SO THAT the company can replenish inventory reliably and maintain an auditable
record of all procurement activity

### SCENARIO 1: Create a new purchase order
**Scenario ID**: PUR-STORY-001-S1

**GIVEN**
* An active vendor with `businessentityid = 10` exists in `purchasing.vendor`
* A valid ship method with `shipmethodid = 1` exists in `purchasing.shipmethod`
* An employee with `businessentityid = 5` exists in `humanresources.employee`

**WHEN**
* A purchasing officer submits a new purchase order for vendor `10`,
  ship method `1`, raised by employee `5`, containing one line item:
  product `316`, quantity `50`, unit price `12.50`

**THEN**
* A new row is inserted into `purchasing.purchaseorderheader` with
  `status = 1` (Pending), `revisionnumber = 1`, and `orderdate` set to
  the current UTC timestamp
* A new row is inserted into `purchasing.purchaseorderdetail` linked to
  the new header, with `orderqty = 50`, `unitprice = 12.50`,
  `receivedqty = 0`, and `rejectedqty = 0`
* The response returns HTTP 201 with the full order including the
  generated `purchaseorderid`

### SCENARIO 2: Retrieve a purchase order with all line items
**Scenario ID**: PUR-STORY-001-S2

**GIVEN**
* A purchase order with `purchaseorderid = 7` exists in
  `purchasing.purchaseorderheader` with two detail lines

**WHEN**
* A GET request is made for purchase order `7`

**THEN**
* The response returns HTTP 200
* The payload includes the header fields (`purchaseorderid`, `status`,
  `vendorid`, `employeeid`, `orderdate`, `subtotal`, `taxamt`, `freight`)
  and a `lineItems` array with both detail rows
* Each line item includes `purchaseorderdetailid`, `productid`, `orderqty`,
  `unitprice`, `receivedqty`, and `rejectedqty`

### SCENARIO 3: Attempt to create a purchase order referencing a non-existent vendor
**Scenario ID**: PUR-STORY-001-S3

**GIVEN**
* No vendor with `businessentityid = 9999` exists in `purchasing.vendor`

**WHEN**
* A POST request is submitted with `vendorid = 9999`

**THEN**
* The API returns HTTP 422 Unprocessable Entity
* The response body contains a validation error:
  `"Vendor 9999 does not exist or is inactive"`
* No rows are inserted into `purchaseorderheader` or `purchaseorderdetail`

### SCENARIO 4: Update the status of a purchase order
**Scenario ID**: PUR-STORY-001-S4

**GIVEN**
* A purchase order with `purchaseorderid = 7` has `status = 1` (Pending)

**WHEN**
* A PATCH request sets `status = 2` (Approved)

**THEN**
* The `status` column in `purchasing.purchaseorderheader` is updated to `2`
* `revisionnumber` is incremented by 1
* `modifieddate` is updated to the current UTC timestamp
* The response returns HTTP 200 with the updated header

### SCENARIO 5: Record received and rejected quantities on a line item
**Scenario ID**: PUR-STORY-001-S5

**GIVEN**
* A purchase order detail with `purchaseorderdetailid = 3` has
  `orderqty = 50`, `receivedqty = 0`, and `rejectedqty = 0`

**WHEN**
* A PATCH request records `receivedqty = 48` and `rejectedqty = 2`

**THEN**
* `receivedqty` is updated to `48` and `rejectedqty` to `2` in
  `purchasing.purchaseorderdetail`
* `modifieddate` is updated to the current UTC timestamp
* The response returns HTTP 200 with the updated line item

---

### PUR-BE-001.1: Purchase Order Header API
**Architecture Reference**: Section 5.4 — Building Block View (Purchasing Schema)

AS A developer
I WANT REST endpoints to create, retrieve, and update purchase order headers
SO THAT the purchasing workflow can be managed over HTTP

#### SCENARIO 1: POST /purchasing/orders creates a new purchase order
**Scenario ID**: PUR-BE-001.1-S1

**GIVEN**
* Vendor `10` and ship method `1` exist in the database
* Employee `5` exists in `humanresources.employee`

**WHEN**
* `POST /purchasing/orders` is called with:
  ```json
  {
    "vendorId": 10,
    "employeeId": 5,
    "shipMethodId": 1,
    "lineItems": [
      { "productId": 316, "orderQty": 50, "unitPrice": 12.50, "dueDate": "2026-10-01T00:00:00Z" }
    ]
  }
  ```

**THEN**
* HTTP 201 is returned
* Response body contains the new `purchaseOrderId`, `status: 1`,
  `revisionNumber: 1`, and the submitted line items
* `Location` header is set to `/purchasing/orders/{purchaseOrderId}`

#### SCENARIO 2: POST /purchasing/orders returns 422 for inactive vendor
**Scenario ID**: PUR-BE-001.1-S2

**GIVEN**
* A vendor with `businessentityid = 20` exists but has `activeflag = false`

**WHEN**
* `POST /purchasing/orders` is called with `vendorId: 20`

**THEN**
* HTTP 422 is returned
* Response body contains:
  ```json
  { "errors": { "vendorId": ["Vendor 20 does not exist or is inactive"] } }
  ```

#### SCENARIO 3: GET /purchasing/orders/{id} returns the full order
**Scenario ID**: PUR-BE-001.1-S3

**GIVEN**
* Purchase order `7` exists with two line items

**WHEN**
* `GET /purchasing/orders/7` is called

**THEN**
* HTTP 200 is returned
* Response body includes `purchaseOrderId: 7`, vendor details, header
  totals (`subtotal`, `taxAmt`, `freight`), and a `lineItems` array
  with both detail rows

#### SCENARIO 4: GET /purchasing/orders/{id} returns 404 when not found
**Scenario ID**: PUR-BE-001.1-S4

**GIVEN**
* No purchase order with `purchaseorderid = 9999` exists

**WHEN**
* `GET /purchasing/orders/9999` is called

**THEN**
* HTTP 404 is returned
* Response body contains a problem details object with
  `title: "Purchase Order Not Found"`

#### SCENARIO 5: PATCH /purchasing/orders/{id}/status updates order status
**Scenario ID**: PUR-BE-001.1-S5

**GIVEN**
* Purchase order `7` has `status = 1` (Pending) and `revisionNumber = 1`

**WHEN**
* `PATCH /purchasing/orders/7/status` is called with:
  ```json
  { "status": 2 }
  ```

**THEN**
* HTTP 200 is returned
* Response body shows `status: 2` and `revisionNumber: 2`
* `modifiedDate` reflects the current UTC timestamp

---

### PUR-BE-001.2: Purchase Order Detail API
**Architecture Reference**: Section 5.4 — Building Block View (Purchasing Schema)

AS A developer
I WANT REST endpoints to record received and rejected quantities on
purchase order line items
SO THAT goods receipt can be tracked per order line

#### SCENARIO 1: PATCH /purchasing/orders/{id}/lines/{lineId} updates receipt quantities
**Scenario ID**: PUR-BE-001.2-S1

**GIVEN**
* Purchase order detail `3` belongs to purchase order `7`
* Current state: `orderQty = 50`, `receivedQty = 0`, `rejectedQty = 0`

**WHEN**
* `PATCH /purchasing/orders/7/lines/3` is called with:
  ```json
  { "receivedQty": 48, "rejectedQty": 2 }
  ```

**THEN**
* HTTP 200 is returned
* Response body shows `receivedQty: 48` and `rejectedQty: 2`
* `modifiedDate` is updated to the current UTC timestamp

#### SCENARIO 2: PATCH rejects receivedQty + rejectedQty exceeding orderQty
**Scenario ID**: PUR-BE-001.2-S2

**GIVEN**
* Purchase order detail `3` has `orderQty = 50`

**WHEN**
* `PATCH /purchasing/orders/7/lines/3` is called with:
  ```json
  { "receivedQty": 48, "rejectedQty": 10 }
  ```

**THEN**
* HTTP 422 is returned
* Response body contains:
  ```json
  { "errors": { "": ["receivedQty + rejectedQty (58) cannot exceed orderQty (50)"] } }
  ```
* No update is persisted to `purchasing.purchaseorderdetail`

#### SCENARIO 3: PATCH returns 404 when line item does not belong to the order
**Scenario ID**: PUR-BE-001.2-S3

**GIVEN**
* Purchase order `7` exists, but detail line `999` belongs to a different order

**WHEN**
* `PATCH /purchasing/orders/7/lines/999` is called

**THEN**
* HTTP 404 is returned
* Response body contains a problem details object with
  `title: "Purchase Order Detail Not Found"`

---

### PUR-BE-001.3: PurchaseOrderHeader and PurchaseOrderDetail EF Core Entities
**Architecture Reference**: Section 9 — Architecture Decisions (ADR-001)

AS A developer
I WANT `PurchaseOrderHeader` and `PurchaseOrderDetail` entities with
`IEntityTypeConfiguration<T>` Fluent API configuration classes
SO THAT EF Core maps the `purchasing` schema tables correctly without
EF Core attributes on the domain entities

#### SCENARIO 1: Entities map to correct schema and table names
**Scenario ID**: PUR-BE-001.3-S1

**GIVEN**
* `PurchaseOrderHeaderConfiguration` and `PurchaseOrderDetailConfiguration`
  are registered in `AdventureWorksDbContext`

**WHEN**
* `dotnet ef migrations add` is run targeting the `purchasing` schema

**THEN**
* The generated migration targets schema `purchasing`
* Table names are `purchaseorderheader` and `purchaseorderdetail`
* `purchaseorderid` is the primary key for the header (identity)
* The composite key `(purchaseorderid, purchaseorderdetailid)` is configured
  on `purchaseorderdetail`
* No EF Core data annotations appear on the domain entity classes

#### SCENARIO 2: Navigation property from header to detail lines is configured
**Scenario ID**: PUR-BE-001.3-S2

**GIVEN**
* `PurchaseOrderHeader` has a collection navigation property `LineItems`
  of type `ICollection<PurchaseOrderDetail>`
* The Fluent API configures a one-to-many relationship with a cascade delete

**WHEN**
* A `PurchaseOrderHeader` is loaded with `.Include(h => h.LineItems)`

**THEN**
* All associated `PurchaseOrderDetail` rows are populated in `LineItems`
* No additional queries are issued (eager loading used)

---

## Pareto Note

PUR-STORY-001 is classified **CORE** because:
- Purchase order management is a primary flow in the Purchasing bounded context
  (Section 1.2)
- It exercises the full architecture stack: API endpoints (BE), EF Core entity
  configuration (BE), EF Core migration (INFRA), FK integrity (INFRA), and
  observability (INFRA)
- `purchaseorderheader.vendorid` → `purchasing.vendor` is an explicit
  cross-schema FK listed in Section 3.3
- It directly delivers visible business value: procurement cannot operate
  without the ability to raise and track purchase orders

---

### PUR-INFRA-001.1: Expose ASP.NET Core Web API endpoint
**Architecture Reference**: Section 3 — Context and Scope

AS A developer
I WANT the purchase order endpoints exposed through `AdventureWorks.Api`
SO THAT purchasing officers can manage purchase orders over HTTP

#### SCENARIO 1: Endpoint responds successfully
**Scenario ID**: PUR-INFRA-001.1-S1

**GIVEN**
* The solution builds successfully (`dotnet build`)
* `AdventureWorks.Api` is running with a valid database connection

**WHEN**
* `GET /purchasing/orders/7` is called for an existing purchase order

**THEN**
* The endpoint returns HTTP 200
* The response `Content-Type` is `application/json`
* The payload includes the purchase order header and its line items

#### SCENARIO 2: POST endpoint creates a purchase order end-to-end
**Scenario ID**: PUR-INFRA-001.1-S2

**GIVEN**
* The solution builds successfully
* Vendor `10`, ship method `1`, and employee `5` exist in the database

**WHEN**
* `POST /purchasing/orders` is called with a valid request body

**THEN**
* HTTP 201 is returned
* The `Location` header points to the newly created resource
* The database contains the new `purchaseorderheader` and at least one
  `purchaseorderdetail` row

#### SCENARIO 3: Non-existent route returns 404
**Scenario ID**: PUR-INFRA-001.1-S3

**GIVEN**
* `AdventureWorks.Api` is running

**WHEN**
* `GET /purchasing/nonexistent` is called

**THEN**
* The API returns HTTP 404 with a standard problem details body

---

### PUR-INFRA-001.2: Apply EF Core migration for purchasing schema
**Architecture Reference**: Section 5.4 — Building Block View (Purchasing Schema) · Section 9 — Architecture Decisions (ADR-001)

AS A developer
I WANT the `purchasing.purchaseorderheader` and `purchasing.purchaseorderdetail`
tables verified and all required entity configurations applied through EF Core
migrations
SO THAT the schema supports purchase order persistence with correct types
and constraints

#### SCENARIO 1: Migration applies cleanly
**Scenario ID**: PUR-INFRA-001.2-S1

**GIVEN**
* PostgreSQL instance is running (AdventureWorks, port 5433)
* The migration for `PUR-STORY-001` has been generated

**WHEN**
* `dotnet ef database update` is executed

**THEN**
* Schema `purchasing` exists
* Table `purchasing.purchaseorderheader` contains the following columns
  with the correct types and nullability:

  | Column | Type | Nullable |
  |--------|------|----------|
  | purchaseorderid | integer (identity) | NOT NULL |
  | revisionnumber | smallint | NOT NULL |
  | status | smallint | NOT NULL |
  | employeeid | integer | NOT NULL |
  | vendorid | integer | NOT NULL |
  | shipmethodid | integer | NOT NULL |
  | orderdate | timestamp without time zone | NOT NULL |
  | shipdate | timestamp without time zone | NULL |
  | subtotal | numeric | NOT NULL |
  | taxamt | numeric | NOT NULL |
  | freight | numeric | NOT NULL |
  | modifieddate | timestamp without time zone | NOT NULL |

* Table `purchasing.purchaseorderdetail` contains the following columns:

  | Column | Type | Nullable |
  |--------|------|----------|
  | purchaseorderid | integer | NOT NULL |
  | purchaseorderdetailid | integer | NOT NULL |
  | duedate | timestamp without time zone | NOT NULL |
  | orderqty | smallint | NOT NULL |
  | productid | integer | NOT NULL |
  | unitprice | numeric | NOT NULL |
  | receivedqty | numeric | NOT NULL |
  | rejectedqty | numeric | NOT NULL |
  | modifieddate | timestamp without time zone | NOT NULL |

* Primary key on `purchaseorderheader` is `purchaseorderid`
* Composite primary key on `purchaseorderdetail` is
  `(purchaseorderid, purchaseorderdetailid)`

#### SCENARIO 2: Round-trip test passes
**Scenario ID**: PUR-INFRA-001.2-S2

**GIVEN**
* The migration has been applied successfully

**WHEN**
* `dotnet ef database update 0` is run (rolls back all migrations)
* `dotnet ef database update` is run again (re-applies all migrations)

**THEN**
* Both operations complete without errors
* Tables `purchasing.purchaseorderheader` and `purchasing.purchaseorderdetail`
  exist with the correct structure after re-application

---

### PUR-INFRA-001.3: Enforce referential integrity
**Architecture Reference**: Section 1.3 — Quality Goals (Priority 1) · Section 3.3 — Cross-Schema Dependencies

AS A developer
I WANT foreign key constraints on `purchasing.purchaseorderheader` enforced
at the database level
SO THAT purchase orders cannot reference non-existent vendors, employees,
or ship methods, and detail lines cannot reference non-existent header rows

#### SCENARIO 1: FK constraint rejects a purchase order for a non-existent vendor
**Scenario ID**: PUR-INFRA-001.3-S1

**GIVEN**
* The migration for `PUR-STORY-001` has been applied
* No vendor with `businessentityid = 9999` exists in `purchasing.vendor`

**WHEN**
* An INSERT is attempted into `purchasing.purchaseorderheader` with
  `vendorid = 9999`

**THEN**
* The database rejects the operation with a foreign key violation error
  referencing `purchaseorderheader.vendorid → vendor.businessentityid`
* No row is inserted into `purchasing.purchaseorderheader`

#### SCENARIO 2: FK constraint rejects a detail line for a non-existent header
**Scenario ID**: PUR-INFRA-001.3-S2

**GIVEN**
* No purchase order with `purchaseorderid = 88888` exists in
  `purchasing.purchaseorderheader`

**WHEN**
* An INSERT is attempted into `purchasing.purchaseorderdetail` with
  `purchaseorderid = 88888`

**THEN**
* The database rejects the operation with a foreign key violation error
  referencing `purchaseorderdetail.purchaseorderid → purchaseorderheader.purchaseorderid`
* No row is inserted into `purchasing.purchaseorderdetail`

#### SCENARIO 3: Deleting a vendor with existing purchase orders is rejected
**Scenario ID**: PUR-INFRA-001.3-S3

**GIVEN**
* Vendor `10` exists in `purchasing.vendor`
* At least one purchase order with `vendorid = 10` exists in
  `purchasing.purchaseorderheader`

**WHEN**
* A DELETE is attempted on `purchasing.vendor` for `businessentityid = 10`

**THEN**
* The database rejects the DELETE with a foreign key violation error
* The vendor row remains in `purchasing.vendor`
* The purchase order rows are not affected

---

### PUR-INFRA-001.4: Setup monitoring and observability
**Architecture Reference**: Section 3.2 — External Interfaces

AS A developer
I WANT structured logging on all purchase order endpoints and a health check
SO THAT I can monitor service health and diagnose procurement issues in production

#### SCENARIO 1: Health check is reachable
**Scenario ID**: PUR-INFRA-001.4-S1

**GIVEN**
* `AdventureWorks.Api` is running with a valid database connection

**WHEN**
* `GET /health` is called

**THEN**
* Response is HTTP 200 OK
* Response body contains `{ "status": "Healthy" }`

#### SCENARIO 2: Structured log entry is emitted on purchase order creation
**Scenario ID**: PUR-INFRA-001.4-S2

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`
  (e.g., Serilog or Microsoft.Extensions.Logging with JSON output)

**WHEN**
* `POST /purchasing/orders` is called and a new purchase order is created

**THEN**
* A structured log entry at `Information` level is emitted containing:
  * A message template identifying the create operation
    (e.g., `"PurchaseOrder {PurchaseOrderId} created for vendor {VendorId}"`)
  * `purchaseOrderId` of the new order
  * `vendorId` and `employeeId` of the order
* No sensitive financial data appears beyond what is needed for traceability

#### SCENARIO 3: Structured log entry is emitted on status update
**Scenario ID**: PUR-INFRA-001.4-S3

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`

**WHEN**
* `PATCH /purchasing/orders/{id}/status` is called and the status is updated

**THEN**
* A structured log entry at `Information` level is emitted containing:
  * `purchaseOrderId`
  * `previousStatus` and `newStatus`
  * `revisionNumber` after the update

#### SCENARIO 4: Structured log entry is emitted at Warning level on validation failure
**Scenario ID**: PUR-INFRA-001.4-S4

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`

**WHEN**
* `POST /purchasing/orders` is called with an invalid `vendorId` and
  returns HTTP 422

**THEN**
* A structured log entry at `Warning` level is emitted containing:
  * The validation error message
  * The offending `vendorId`
* The log entry does not include stack trace details (validation failures
  are expected paths, not exceptions)
