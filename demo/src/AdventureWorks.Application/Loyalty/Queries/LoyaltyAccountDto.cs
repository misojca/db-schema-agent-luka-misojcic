namespace AdventureWorks.Application.Loyalty.Queries;

/// <summary>
/// Read model returned by <see cref="GetLoyaltyAccountByCustomerIdQuery"/>.
/// Exposes the fields served by GET /api/loyalty/{customerId}.
/// LOY-INFRA-001.2
/// </summary>
public record LoyaltyAccountDto(
    int LoyaltyAccountId,
    int CustomerId,
    int PointsBalance,
    DateTime CreatedAt,
    DateTime ModifiedAt);
