namespace AdventureWorks.Application.Employees.Queries;

/// <summary>
/// Read model returned by <see cref="GetAllEmployeesQuery"/>.
/// Exposes the fields served by GET /api/employees, including the
/// pay-denormalisation columns added in issue #7 (Scenario 6).
/// </summary>
public record EmployeeDto(
    int BusinessEntityId,
    string JobTitle,
    DateOnly HireDate,
    decimal? CurrentPayRate,
    short? CurrentPayFrequency,
    DateTime? PayChangeDate);
