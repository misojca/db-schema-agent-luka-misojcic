namespace AdventureWorks.Domain.Loyalty.Entities;

/// <summary>
/// Customer loyalty account tracking points balance.
/// Maps to loyalty.loyaltyaccount.
/// Cross-schema FK: customerid → sales.customer.customerid
/// Added in issue #5 (Scenario 4).
/// </summary>
public class LoyaltyAccount
{
    /// <summary>Primary key.</summary>
    public int LoyaltyAccountId { get; set; }

    /// <summary>FK to sales.customer. Each customer has at most one loyalty account.</summary>
    public int CustomerId { get; set; }

    /// <summary>Current accumulated points balance. Cannot be negative.</summary>
    public int PointsBalance { get; set; }

    /// <summary>Date the loyalty account was created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Date the loyalty account record was last updated.</summary>
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Optional expiry date of the customer's current loyalty tier.
    /// NULL means no tier is active or the tier does not expire.
    /// Added in issue #25 (Scenario 8).
    /// </summary>
    public DateTime? TierExpiryDate { get; set; }
}
