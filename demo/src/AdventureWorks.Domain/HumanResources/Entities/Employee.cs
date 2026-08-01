namespace AdventureWorks.Domain.HumanResources.Entities;

/// <summary>
/// Employee information.
/// Maps to humanresources.employee.
/// Issue #7 (Scenario 6): currentpayrate, currentpayfrequency, paychangedate added
/// via expand-contract migration, populated from employeepayhistory.
/// </summary>
public class Employee
{
    public int BusinessEntityId { get; set; }
    public string NationalIdNumber { get; set; } = string.Empty;
    public string LoginId { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public string MaritalStatus { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateOnly HireDate { get; set; }
    public bool SalariedFlag { get; set; }
    public short VacationHours { get; set; }
    public short SickLeaveHours { get; set; }
    public bool CurrentFlag { get; set; }
    public Guid Rowguid { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? OrganizationNode { get; set; }

    // --- Issue #7: pay denormalisation columns ---
    // Nullable during Phase 1 expand; made NOT NULL in Phase 2 contract
    // after data migration from employeepayhistory.

    /// <summary>Most recent pay rate from employeepayhistory.rate.</summary>
    public decimal? CurrentPayRate { get; set; }

    /// <summary>Most recent pay frequency from employeepayhistory.payfrequency (1=monthly, 2=bi-weekly).</summary>
    public short? CurrentPayFrequency { get; set; }

    /// <summary>Date of the most recent pay change from employeepayhistory.ratechangedate.</summary>
    public DateTime? PayChangeDate { get; set; }
}
