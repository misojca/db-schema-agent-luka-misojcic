using AdventureWorks.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Infrastructure.Persistence;

public class AdventureWorksDbContext : DbContext
{
    public AdventureWorksDbContext(DbContextOptions<AdventureWorksDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AdventureWorksDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
