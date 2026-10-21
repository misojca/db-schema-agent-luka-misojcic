using AdventureWorks.Domain.Person.Entities;

namespace AdventureWorks.Domain.Person.Entities;

/// <summary>
/// Street address information for customers, employees, and vendors.
/// Maps to person.address.
/// Issue #6 (Scenario 5): spatiallocation moved to person.addressdetail.
/// </summary>
public class Address
{
    public int AddressId { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public int StateProvinceId { get; set; }
    public string PostalCode { get; set; } = string.Empty;

    public Guid Rowguid { get; set; }
    public DateTime ModifiedDate { get; set; }

    // Navigation: one-to-one with AddressDetail (spatiallocation moved here in issue #6)
    public AddressDetail? AddressDetail { get; set; }
}
