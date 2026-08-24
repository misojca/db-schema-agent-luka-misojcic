using AdventureWorks.Application.Interfaces;
using AdventureWorks.Domain.HumanResources.Entities;
using AdventureWorks.Domain.Loyalty.Entities;
using AdventureWorks.Domain.Person.Entities;
using AdventureWorks.Domain.Production.Entities;
using AdventureWorks.Domain.Purchasing.Entities;
using AdventureWorks.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Infrastructure.Persistence;

public class AdventureWorksDbContext : DbContext, IApplicationDbContext
{
    public AdventureWorksDbContext(DbContextOptions<AdventureWorksDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<PurchaseReturn> PurchaseReturns => Set<PurchaseReturn>();
    public DbSet<LoyaltyAccount> LoyaltyAccounts => Set<LoyaltyAccount>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<AddressDetail> AddressDetails => Set<AddressDetail>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<SpecialOffer> SpecialOffers => Set<SpecialOffer>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AdventureWorksDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
