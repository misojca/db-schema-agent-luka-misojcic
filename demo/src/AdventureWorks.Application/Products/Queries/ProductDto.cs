namespace AdventureWorks.Application.Products.Queries;

/// <summary>
/// Read model returned by <see cref="GetAllProductsQuery"/>.
/// Contains the fields exposed by GET /api/products.
/// </summary>
public record ProductDto(
    int ProductId,
    string Name,
    string ProductNumber,
    decimal ListPrice);
