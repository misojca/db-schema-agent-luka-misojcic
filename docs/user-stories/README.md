# User Story Inventory — AdventureWorks

## Bounded Contexts

| Prefix | Schema | Responsibility |
|--------|--------|----------------|
| **HR** | humanresources | Employees, departments, pay history, job candidates |
| **PER** | person | Contacts, addresses, phone numbers, business entities |
| **PROD** | production | Products, categories, bill of materials, inventory |
| **PUR** | purchasing | Vendors, purchase orders, ship methods |
| **SALES** | sales | Customers, orders, territories, currencies, promotions |
| **LOY** | loyalty | Customer loyalty accounts and points balances |

---

## Pareto Analysis

Target: ~20% of stories (CORE) deliver ~80% of system value.

A story is **CORE** if it:
- Is required for core reporting/ordering flows (Sales, Purchasing)
- Blocks other stories from being completed
- Covers a primary bounded context (HumanResources, Person, Production, Purchasing, Sales)
- Exercises multiple architecture components (API + DB + FK integrity)
- Delivers direct, visible value to the end user

A story is **SUPPORTING** if it:
- Enhances an existing core feature (filtering, sorting, pagination)
- Touches a smaller/peripheral schema (e.g., Loyalty)
- Is a nice-to-have that adds convenience but is not required for MVP
- Can be deferred without blocking other work

### CORE Stories — 20% (critical path, MVP blockers)

| ID | Title | Domain File |
|----|-------|-------------|
| *(pending)* | Sales order management | sales.md |
| *(pending)* | Customer management | sales.md |
| *(pending)* | Product catalog | production.md |
| *(pending)* | Purchase order management | purchasing.md |
| *(pending)* | Employee management | humanresources.md |

### SUPPORTING Stories — 80% (enhancements, not MVP blockers)

| ID | Title | Domain File |
|----|-------|-------------|
| LOY-STORY-001 | Manage Customer Loyalty Account | loyalty.md |

---

## Story Inventory

| Story ID | Title | Priority | Status |
|----------|-------|----------|--------|
| LOY-STORY-001 | Manage Customer Loyalty Account | [CORE]* | ⬜ Pending Approval |

> \* Marked CORE per explicit request. By schema size and peripheral nature,
> Loyalty would normally be SUPPORTING. It is promoted to CORE here as the
> first story in this inventory and as the anchor for cross-schema FK
> integrity validation (Quality Goal #1).

---

## Pareto Progress

📊 Pareto Progress: 0/0 core stories complete (— % of 20% core stories)
🎯 Core functionality coverage: ~0% of 80% target

> Progress will be updated as stories are approved.
