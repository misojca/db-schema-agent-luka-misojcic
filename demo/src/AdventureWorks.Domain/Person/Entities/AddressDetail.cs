namespace AdventureWorks.Domain.Person.Entities;

/// <summary>
/// Spatial detail for an address, split from person.address in issue #6 (Scenario 5).
/// Maps to person.addressdetail.
/// One-to-one with person.address via addressid FK/PK.
/// </summary>
public class AddressDetail
{
    /// <summary>PK and FK to person.address. One detail row per address.</summary>
    public int AddressId { get; set; }

    /// <summary>
    /// Spatial location data moved from person.address.spatiallocation.
    /// Nullable during Phase 1 (expand); all rows populated before Phase 2 (contract).
    /// </summary>
    public byte[]? SpatialLocation { get; set; }

    public DateTime ModifiedDate { get; set; }
}
