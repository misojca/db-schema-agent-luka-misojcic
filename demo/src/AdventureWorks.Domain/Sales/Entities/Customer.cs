namespace AdventureWorks.Domain.Sales.Entities;

public class Customer
{
    public int CustomerId { get; private set; }
    public int? PersonId { get; private set; }
    public int? StoreId { get; private set; }
    public int? TerritoryId { get; private set; }
    public Guid Rowguid { get; private set; }
    public DateTime ModifiedDate { get; private set; }

    private Customer() { }

    public static Customer Create(int? personId, int? storeId, int? territoryId)
    {
        return new Customer
        {
            PersonId = personId,
            StoreId = storeId,
            TerritoryId = territoryId,
            Rowguid = Guid.NewGuid(),
            ModifiedDate = DateTime.UtcNow
        };
    }
}
