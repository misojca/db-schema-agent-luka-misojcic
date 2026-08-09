using MediatR;

namespace AdventureWorks.Application.Products.Queries;

/// <summary>
/// Query: retrieve all products from the production.product table.
/// Returns a list of <see cref="ProductDto"/>.
/// </summary>
public record GetAllProductsQuery : IRequest<List<ProductDto>>;
