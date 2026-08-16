using MediatR;

namespace AdventureWorks.Application.Loyalty.Queries;

/// <summary>
/// Query: retrieve a loyalty account by customer ID from loyalty.loyaltyaccount.
/// Returns the <see cref="LoyaltyAccountDto"/> or null if not found.
/// LOY-INFRA-001.2
/// </summary>
public record GetLoyaltyAccountByCustomerIdQuery(int CustomerId)
    : IRequest<LoyaltyAccountDto?>;
