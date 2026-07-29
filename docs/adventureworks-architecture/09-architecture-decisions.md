# 9. Architecture Decisions

## ADR-001: Use EF Core with Fluent API for schema configuration

**Status:** Accepted

**Context:**
The DB Schema Agent generates C# code for .NET 8 / EF Core 8. Schema
configuration can be done via Data Annotations or Fluent API.

**Decision:**
Use Fluent API exclusively via IEntityTypeConfiguration<T> classes.

**Rationale:**
- Keeps domain entities clean (no EF Core attributes in Domain project)
- Supports Clean Architecture — Infrastructure concerns stay in Infrastructure
- More expressive for complex mappings (composite keys, schemas, indexes)

**Consequences:**
- Every entity requires a corresponding configuration class
- Slightly more verbose than Data Annotations

## ADR-002: One DbContext per bounded context (future)

**Status:** Proposed

**Context:**
Currently all entities share AdventureWorksDbContext. As the system grows,
cross-context queries become harder to manage.

**Decision:**
Split into per-bounded-context DbContexts (SalesDbContext, ProductionDbContext,
etc.) in a future iteration.

**Rationale:**
- Aligns with DDD bounded context principle
- Reduces DbContext size and complexity
- Enables independent scaling of contexts

**Consequences:**
- Cross-schema queries require explicit joins or API calls between contexts
- Migration management becomes more complex

## ADR-003: Expand-Contract pattern for breaking schema changes

**Status:** Accepted

**Context:**
Changing existing column types or splitting tables risks data loss in
production environments.

**Decision:**
All breaking schema changes use expand-contract pattern:
1. Add new structure (expand)
2. Migrate data
3. Remove old structure (contract)

**Rationale:**
- Zero downtime deployments
- Data preservation guaranteed
- Backward compatible during transition

**Consequences:**
- Breaking changes require multiple migration files
- Transition period where both old and new structure coexist
