using AdventureWorks.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Application.Loyalty.Queries;

/// <summary>
/// Handles <see cref="GetLoyaltyAccountByCustomerIdQuery"/> by querying
/// loyalty.loyaltyaccount and projecting the result into a <see cref="LoyaltyAccountDto"/>.
/// Returns null when no account exists for the given customer.
/// LOY-INFRA-001.2
/// </summary>
public class GetLoyaltyAccountByCustomerIdQueryHandler
    : IRequestHandler<GetLoyaltyAccountByCustomerIdQuery, LoyaltyAccountDto?>
{
    private readonly IApplicationDbContext _db;

    public GetLoyaltyAccountByCustomerIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<LoyaltyAccountDto?> Handle(
        GetLoyaltyAccountByCustomerIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _db.LoyaltyAccounts
            .AsNoTracking()
            .Where(l => l.CustomerId == request.CustomerId)
            .Select(l => new LoyaltyAccountDto(
                l.LoyaltyAccountId,
                l.CustomerId,
                l.PointsBalance,
                l.CreatedAt,
                l.ModifiedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
