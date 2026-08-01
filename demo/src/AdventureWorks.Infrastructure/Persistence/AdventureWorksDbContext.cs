using AdventureWorks.Domain.Loyalty.Entities;
using AdventureWorks.Domain.Person.Entities;
using AdventureWorks.Domain.Purchasing.Entities;
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
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
    public DbSet<LoyaltyAccount> LoyaltyAccounts => Set<LoyaltyAccount>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<AddressDetail> AddressDetails => Set<AddressDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AdventureWorksDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
