namespace AdventureWorks.Domain.Sales.Entities;

public class SalesTerritory
{
    public int TerritoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public string CountryRegionCode { get; private set; } = null!;
    public string Group { get; private set; } = null!;
    public decimal SalesYtd { get; private set; }
    public decimal SalesLastYear { get; private set; }
    public decimal CostYtd { get; private set; }
    public decimal CostLastYear { get; private set; }
    public Guid Rowguid { get; private set; }
    public DateTime ModifiedDate { get; private set; }

    private SalesTerritory() { }

    public static SalesTerritory Create(
        string name,
        string countryRegionCode,
        string group,
        decimal salesYtd = 0m,
        decimal salesLastYear = 0m,
        decimal costYtd = 0m,
        decimal costLastYear = 0m)
    {
        return new SalesTerritory
        {
            Name = name,
            CountryRegionCode = countryRegionCode,
            Group = group,
            SalesYtd = salesYtd,
            SalesLastYear = salesLastYear,
            CostYtd = costYtd,
            CostLastYear = costLastYear,
            Rowguid = Guid.NewGuid(),
            ModifiedDate = DateTime.UtcNow
        };
    }
}
