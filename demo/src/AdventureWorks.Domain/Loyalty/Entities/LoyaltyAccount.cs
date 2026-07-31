namespace AdventureWorks.Domain.Loyalty.Entities;

public class LoyaltyAccount
{
    public int LoyaltyAccountId { get; set; }
    public int CustomerId { get; set; }
    public int PointsBalance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
