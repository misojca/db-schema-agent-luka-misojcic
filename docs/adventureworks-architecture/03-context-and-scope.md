# 3. Context and Scope

## 3.1 System Boundary
The AdventureWorks PostgreSQL database contains 68 tables across 5 logical
schemas. The DB Schema Agent operates on this database — it reads the
existing schema, understands the domain model, and generates incremental
EF Core migrations in response to change requests.

## 3.2 External Interfaces
| System | Interaction |
|--------|------------|
| AdventureWorks PostgreSQL | Source of truth — agent reads and modifies schema |
| EF Core Migrations | Output artifact — agent generates C# migration files |
| GitHub Actions CI | Runs migration round-trip tests on every push |
| Kiro CLI | Agent runtime — executes shell commands and writes files |

## 3.3 Cross-Schema Dependencies

| FK | From | To |
|----|------|----|
| customer.personid | sales | person.person |
| customer.storeid | sales | sales.store |
| customer.territoryid | sales | sales.salesterritory |
| salesorderheader.customerid | sales | sales.customer |
| employee.businessentityid | humanresources | person.person |
| purchaseorderheader.vendorid | purchasing | purchasing.vendor |
| product.productsubcategoryid | production | production.productsubcategory |
