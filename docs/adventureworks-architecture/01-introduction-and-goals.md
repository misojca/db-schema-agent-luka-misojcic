# 1. Introduction and Goals

## 1.1 Purpose
AdventureWorks is a manufacturing and sales company database that tracks
products, customers, orders, purchasing, and human resources. This
architecture document describes the existing PostgreSQL schema and serves
as input for the DB Schema Agent to understand the domain and generate
EF Core entity classes and migrations.

## 1.2 Bounded Contexts

| Context | Schema | Responsibility |
|---------|--------|---------------|
| **HumanResources** | humanresources | Employees, departments, pay history, job candidates |
| **Person** | person | Contacts, addresses, phone numbers, business entities |
| **Production** | production | Products, categories, bill of materials, inventory |
| **Purchasing** | purchasing | Vendors, purchase orders, ship methods |
| **Sales** | sales | Customers, orders, territories, currencies, promotions |

## 1.3 Quality Goals

| Priority | Quality Attribute | Scenario |
|----------|------------------|----------|
| 1 | Referential Integrity | All cross-schema FK constraints enforced at database level |
| 2 | Backward Compatibility | Schema changes must not break existing data or queries |
| 3 | Traceability | Every migration references the story or requirement that motivated it |
| 4 | Testability | Every schema change validated by round-trip migration test |
