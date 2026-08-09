using AdventureWorks.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Application.Products.Queries;

/// <summary>
/// Handles <see cref="GetAllProductsQuery"/> by querying the database
/// and projecting each row into a <see cref="ProductDto"/>.
/// </summary>
public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAllProductsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductDto>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.Products
            .AsNoTracking()
            .Select(p => new ProductDto(
                p.ProductId,
                p.Name,
                p.ProductNumber,
                p.ListPrice))
            .ToListAsync(cancellationToken);
    }
}
