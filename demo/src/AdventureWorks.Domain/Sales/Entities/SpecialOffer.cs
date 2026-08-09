namespace AdventureWorks.Domain.Sales.Entities;

public class SpecialOffer
{
    public int SpecialOfferId { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal DiscountPct { get; private set; }
    public string Type { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public int MinQty { get; private set; }
    public int? MaxQty { get; private set; }
    public Guid Rowguid { get; private set; }
    public DateTime ModifiedDate { get; private set; }

    /// <summary>
    /// Nullable discount percentage stored as numeric(5,2).
    /// Added in migration AddDiscountPercentageToSpecialOffer.
    /// Defaults to 0.00 when not explicitly set.
    /// </summary>
    public decimal? DiscountPercentage { get; private set; }

    private SpecialOffer() { }

    public static SpecialOffer Create(
        string description,
        decimal discountPct,
        string type,
        string category,
        DateTime startDate,
        DateTime endDate,
        int minQty,
        int? maxQty,
        decimal? discountPercentage = null)
    {
        return new SpecialOffer
        {
            Description = description,
            DiscountPct = discountPct,
            Type = type,
            Category = category,
            StartDate = startDate,
            EndDate = endDate,
            MinQty = minQty,
            MaxQty = maxQty,
            Rowguid = Guid.NewGuid(),
            ModifiedDate = DateTime.UtcNow,
            DiscountPercentage = discountPercentage
        };
    }
}
