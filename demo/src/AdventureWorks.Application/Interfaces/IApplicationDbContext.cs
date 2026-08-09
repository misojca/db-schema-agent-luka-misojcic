using AdventureWorks.Domain.HumanResources.Entities;
using AdventureWorks.Domain.Production.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Application.Interfaces;

/// <summary>
/// Abstraction over the database context exposed to the Application layer.
/// Keeps Application free of a direct EF Core / Infrastructure dependency.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Employee> Employees { get; }
}
