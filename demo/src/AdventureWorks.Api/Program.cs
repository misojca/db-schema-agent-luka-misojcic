using AdventureWorks.Application.Interfaces;
using AdventureWorks.Application.Products.Queries;
using AdventureWorks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AdventureWorksDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__ef_migrations_history", "public")
    ));

// Register the DB context as IApplicationDbContext so MediatR handlers can depend on the abstraction.
builder.Services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<AdventureWorksDbContext>());

// Register MediatR and scan the Application assembly for all IRequestHandlers.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(GetAllProductsQuery).Assembly));

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

// Expose Program class for WebApplicationFactory in integration tests.
public partial class Program { }
