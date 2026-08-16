# Loyalty Domain — User Stories

---

## LOY-STORY-001: Manage Customer Loyalty Account

**Architecture Reference**: Section 5.6 — Building Block View (Loyalty Schema)
**Priority**: CORE
**Pareto**: 20% core (delivers 80% value)

AS A customer
I WANT a loyalty account that tracks my points balance
SO THAT I can accumulate points through purchases and redeem them for rewards

### SCENARIO 1: Retrieve loyalty account balance
**Scenario ID**: LOY-STORY-001-S1

**GIVEN**
* A customer with `customerid = 42` exists in `sales.customer`
* A loyalty account with `customerid = 42` exists in `loyalty.loyaltyaccount`
  with `points_balance = 150`

**WHEN**
* The customer requests their loyalty account details

**THEN**
* The response includes `loyaltyaccountid`, `customerid`, `points_balance`,
  `created_at`, and `modified_at`
* `points_balance` is returned as `150`

### SCENARIO 2: Create a new loyalty account for an existing customer
**Scenario ID**: LOY-STORY-001-S2

**GIVEN**
* A customer with `customerid = 99` exists in `sales.customer`
* No loyalty account exists yet for `customerid = 99`

**WHEN**
* A POST request is submitted to create a loyalty account for `customerid = 99`

**THEN**
* A new row is inserted into `loyalty.loyaltyaccount`
* `points_balance` defaults to `0`
* `created_at` and `modified_at` are set to the current UTC timestamp
* The response returns HTTP 201 with the new account's details

### SCENARIO 3: Update (credit) points balance
**Scenario ID**: LOY-STORY-001-S3

**GIVEN**
* A loyalty account for `customerid = 42` exists with `points_balance = 150`

**WHEN**
* A PATCH request credits `50` points to the account

**THEN**
* `points_balance` is updated to `200`
* `modified_at` is updated to the current UTC timestamp
* The response returns HTTP 200 with the updated account

### SCENARIO 4: Attempt to create a duplicate loyalty account
**Scenario ID**: LOY-STORY-001-S4

**GIVEN**
* A loyalty account for `customerid = 42` already exists

**WHEN**
* A POST request attempts to create a second loyalty account for `customerid = 42`

**THEN**
* The API returns HTTP 409 Conflict
* No new row is inserted into `loyalty.loyaltyaccount`

---

### LOY-BE-001.1: Loyalty Account CRUD API
**Architecture Reference**: Section 5.6 — Building Block View (Loyalty Schema)

AS A developer
I WANT REST endpoints to create, read, and update loyalty accounts
SO THAT the system can manage customer points balances via HTTP

#### SCENARIO 1: GET /loyalty/accounts/{customerId} returns account details
**Scenario ID**: LOY-BE-001.1-S1

**GIVEN**
* A loyalty account for `customerId = 42` exists with `points_balance = 150`

**WHEN**
* `GET /loyalty/accounts/42` is called

**THEN**
* HTTP 200 is returned
* Response body contains:
  ```json
  {
    "loyaltyAccountId": <integer>,
    "customerId": 42,
    "pointsBalance": 150,
    "createdAt": "<ISO 8601 timestamp>",
    "modifiedAt": "<ISO 8601 timestamp>"
  }
  ```

#### SCENARIO 2: GET /loyalty/accounts/{customerId} returns 404 when not found
**Scenario ID**: LOY-BE-001.1-S2

**GIVEN**
* No loyalty account exists for `customerId = 999`

**WHEN**
* `GET /loyalty/accounts/999` is called

**THEN**
* HTTP 404 is returned
* Response body contains a problem details object with `title: "Not Found"`

#### SCENARIO 3: POST /loyalty/accounts creates a new account
**Scenario ID**: LOY-BE-001.1-S3

**GIVEN**
* A customer with `customerId = 99` exists in `sales.customer`
* No loyalty account exists for `customerId = 99`

**WHEN**
* `POST /loyalty/accounts` is called with body:
  ```json
  { "customerId": 99 }
  ```

**THEN**
* HTTP 201 is returned
* Response body contains the newly created account with `pointsBalance: 0`
* `Location` header is set to `/loyalty/accounts/99`

#### SCENARIO 4: POST /loyalty/accounts returns 409 for duplicate
**Scenario ID**: LOY-BE-001.1-S4

**GIVEN**
* A loyalty account for `customerId = 42` already exists

**WHEN**
* `POST /loyalty/accounts` is called with body:
  ```json
  { "customerId": 42 }
  ```

**THEN**
* HTTP 409 Conflict is returned
* No row is inserted into `loyalty.loyaltyaccount`

#### SCENARIO 5: PATCH /loyalty/accounts/{customerId}/points credits points
**Scenario ID**: LOY-BE-001.1-S5

**GIVEN**
* A loyalty account for `customerId = 42` exists with `pointsBalance = 150`

**WHEN**
* `PATCH /loyalty/accounts/42/points` is called with body:
  ```json
  { "delta": 50 }
  ```

**THEN**
* HTTP 200 is returned
* Response body shows `pointsBalance: 200`
* `modifiedAt` reflects the current UTC timestamp

#### SCENARIO 6: PATCH /loyalty/accounts/{customerId}/points rejects negative balance
**Scenario ID**: LOY-BE-001.1-S6

**GIVEN**
* A loyalty account for `customerId = 42` exists with `pointsBalance = 30`

**WHEN**
* `PATCH /loyalty/accounts/42/points` is called with body:
  ```json
  { "delta": -100 }
  ```

**THEN**
* HTTP 422 Unprocessable Entity is returned
* Response body contains a validation error: `"Resulting points balance cannot be negative"`
* `points_balance` remains `30` in the database

---

### LOY-BE-001.2: LoyaltyAccount EF Core Entity and Configuration
**Architecture Reference**: Section 9 — Architecture Decisions (ADR-001)

AS A developer
I WANT a `LoyaltyAccount` entity with an `IEntityTypeConfiguration<LoyaltyAccount>`
Fluent API configuration class
SO THAT EF Core maps the `loyalty.loyaltyaccount` table correctly without
polluting the domain entity with EF Core attributes

#### SCENARIO 1: Entity maps to correct schema and table
**Scenario ID**: LOY-BE-001.2-S1

**GIVEN**
* `LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>`
  is registered in `AdventureWorksDbContext`

**WHEN**
* `dotnet ef migrations add` is run against the `loyalty` schema

**THEN**
* The generated migration targets schema `loyalty`, table `loyaltyaccount`
* `loyaltyaccountid` is configured as the primary key with identity
* `customerid`, `points_balance`, `created_at`, and `modified_at` are
  mapped with correct column names and types
* No EF Core data annotations appear on the `LoyaltyAccount` domain class

#### SCENARIO 2: Unique constraint on customerid is enforced at model level
**Scenario ID**: LOY-BE-001.2-S2

**GIVEN**
* The `LoyaltyAccountConfiguration` configures a unique index on `customerid`

**WHEN**
* A second `LoyaltyAccount` with an existing `customerid` is added and
  `SaveChanges()` is called

**THEN**
* The database rejects the insert with a unique constraint violation
* The EF Core exception propagates as a `DbUpdateException`

---

### LOY-INFRA-001.1: Expose ASP.NET Core Web API endpoint
**Architecture Reference**: Section 3 — Context and Scope

AS A developer
I WANT the loyalty endpoints exposed through `AdventureWorks.Api`
SO THAT clients can consume loyalty account operations over HTTP

#### SCENARIO 1: Endpoint responds successfully
**Scenario ID**: LOY-INFRA-001.1-S1

**GIVEN**
* The solution builds successfully (`dotnet build`)
* `AdventureWorks.Api` is running with a valid database connection

**WHEN**
* `GET /loyalty/accounts/42` is called with a valid `customerId`

**THEN**
* The endpoint returns HTTP 200 with the loyalty account payload
* The response `Content-Type` is `application/json`

#### SCENARIO 2: Non-existent route returns 404
**Scenario ID**: LOY-INFRA-001.1-S2

**GIVEN**
* `AdventureWorks.Api` is running

**WHEN**
* `GET /loyalty/nonexistent` is called

**THEN**
* The API returns HTTP 404 with a standard problem details body

---

### LOY-INFRA-001.2: Apply EF Core migration for loyalty schema
**Architecture Reference**: Section 5.6 — Building Block View (Loyalty Schema) · Section 9 — Architecture Decisions (ADR-001)

AS A developer
I WANT the `loyalty.loyaltyaccount` table created through an EF Core migration
SO THAT the schema supports loyalty account persistence

#### SCENARIO 1: Migration applies cleanly
**Scenario ID**: LOY-INFRA-001.2-S1

**GIVEN**
* PostgreSQL instance is running (AdventureWorks, port 5433)
* The migration for `LOY-STORY-001` has been generated

**WHEN**
* `dotnet ef database update` is executed

**THEN**
* Schema `loyalty` exists (created if absent)
* Table `loyalty.loyaltyaccount` exists with columns:
  | Column | Type | Nullable |
  |--------|------|----------|
  | loyaltyaccountid | integer (identity) | NOT NULL |
  | customerid | integer | NOT NULL |
  | points_balance | integer | NOT NULL |
  | created_at | timestamp without time zone | NOT NULL |
  | modified_at | timestamp without time zone | NOT NULL |
* A unique index exists on `customerid`
* Primary key is `loyaltyaccountid`

#### SCENARIO 2: Round-trip test passes
**Scenario ID**: LOY-INFRA-001.2-S2

**GIVEN**
* The migration has been applied successfully

**WHEN**
* `dotnet ef database update 0` is run (rolls back all migrations)
* `dotnet ef database update` is run again (re-applies all migrations)

**THEN**
* Both operations complete without errors
* Table `loyalty.loyaltyaccount` exists with the correct structure after
  re-application

---

### LOY-INFRA-001.3: Enforce referential integrity
**Architecture Reference**: Section 1.3 — Quality Goals (Priority 1) · Section 3.3 — Cross-Schema Dependencies

AS A developer
I WANT a foreign key constraint from `loyalty.loyaltyaccount.customerid`
to `sales.customer.customerid` enforced at the database level
SO THAT a loyalty account cannot reference a non-existent customer

#### SCENARIO 1: FK constraint rejects orphaned loyalty account
**Scenario ID**: LOY-INFRA-001.3-S1

**GIVEN**
* The migration for `LOY-STORY-001` has been applied
* No customer with `customerid = 9999` exists in `sales.customer`

**WHEN**
* An INSERT is attempted into `loyalty.loyaltyaccount` with `customerid = 9999`

**THEN**
* The database rejects the operation with a foreign key violation error
* No row is inserted into `loyalty.loyaltyaccount`

#### SCENARIO 2: Deleting a customer with an active loyalty account is rejected
**Scenario ID**: LOY-INFRA-001.3-S2

**GIVEN**
* A customer with `customerid = 42` exists in `sales.customer`
* A loyalty account with `customerid = 42` exists in `loyalty.loyaltyaccount`

**WHEN**
* A DELETE is attempted on `sales.customer` for `customerid = 42`

**THEN**
* The database rejects the DELETE with a foreign key violation error
* The customer row remains in `sales.customer`
* The loyalty account row remains in `loyalty.loyaltyaccount`

---

### LOY-INFRA-001.4: Setup monitoring and observability
**Architecture Reference**: Section 3.2 — External Interfaces

AS A developer
I WANT structured logging on all loyalty endpoints and a health check
SO THAT I can monitor service health and diagnose issues in production

#### SCENARIO 1: Health check is reachable
**Scenario ID**: LOY-INFRA-001.4-S1

**GIVEN**
* `AdventureWorks.Api` is running with a valid database connection

**WHEN**
* `GET /health` is called

**THEN**
* Response is HTTP 200 OK
* Response body contains `{ "status": "Healthy" }`

#### SCENARIO 2: Structured log entry is emitted on loyalty account creation
**Scenario ID**: LOY-INFRA-001.4-S2

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`
  (e.g., Serilog or Microsoft.Extensions.Logging with JSON output)

**WHEN**
* `POST /loyalty/accounts` is called and a new account is created

**THEN**
* A structured log entry at `Information` level is emitted containing:
  * `EventId` or message template identifying the create operation
  * `customerId` of the new account
  * The assigned `loyaltyAccountId`
* No sensitive data (e.g., credit card numbers, passwords) appears in the log

#### SCENARIO 3: Structured log entry is emitted on points update
**Scenario ID**: LOY-INFRA-001.4-S3

**GIVEN**
* Structured logging is configured in `AdventureWorks.Api`

**WHEN**
* `PATCH /loyalty/accounts/{customerId}/points` is called

**THEN**
* A structured log entry at `Information` level is emitted containing:
  * `customerId`
  * `delta` applied
  * `newPointsBalance` after the update
