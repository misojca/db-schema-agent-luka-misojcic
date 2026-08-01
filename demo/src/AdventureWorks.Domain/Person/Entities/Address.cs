namespace AdventureWorks.Domain.Person.Entities;

/// <summary>
/// Street address information for customers, employees, and vendors.
/// Maps to person.address.
/// Note: spatiallocation column is being moved to AddressDetail in issue #6 (Scenario 5).
/// </summary>
public class Address
{
    public int AddressId { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public int StateProvinceId { get; set; }
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Spatial location. Phase 1: still present on address.
    /// Phase 2 (contract): this column is dropped after data migrated to addressdetail.
    /// </summary>
    public byte[]? SpatialLocation { get; set; }

    public Guid Rowguid { get; set; }
    public DateTime ModifiedDate { get; set; }
}
