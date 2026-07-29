using AdventureWorks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AdventureWorks.Integration.Tests;

public class DatabaseConnectionTests
{
    private readonly AdventureWorksDbContext _context;

    public DatabaseConnectionTests()
    {
        var options = new DbContextOptionsBuilder<AdventureWorksDbContext>()
            .UseNpgsql("Host=localhost;Port=5433;Database=Adventureworks;Username=postgres;Password=postgres")
            .Options;

        _context = new AdventureWorksDbContext(options);
    }

    [Fact]
    public async Task CanConnectToDatabase()
    {
        var canConnect = await _context.Database.CanConnectAsync();
        Assert.True(canConnect);
    }

    [Fact]
    public async Task CanReadCustomersFromSalesSchema()
    {
        var customers = await _context.Customers
            .Take(5)
            .ToListAsync();

        Assert.NotNull(customers);
        Assert.True(customers.Count > 0);
    }
}
