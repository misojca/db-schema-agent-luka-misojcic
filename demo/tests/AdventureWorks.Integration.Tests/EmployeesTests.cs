using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AdventureWorks.Integration.Tests;

/// <summary>
/// Integration tests for GET /api/employees — issue #17.
/// Uses WebApplicationFactory to spin up the full ASP.NET Core pipeline in-process.
/// </summary>
public class EmployeesTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EmployeesTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Given_EmployeesExist_When_GetAllEmployees_Then_ReturnsOkWithNonEmptyList()
    {
        // GIVEN - the humanresources.employee table contains rows (seeded in the live DB)

        // WHEN - GET /api/employees is called
        var response = await _client.GetAsync("/api/employees");

        // THEN - 200 OK is returned with a non-empty JSON array
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var employees = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
        Assert.NotNull(employees);
        Assert.True(employees.Count > 0);
    }

    [Fact]
    public async Task Given_EmployeesExist_When_GetAllEmployees_Then_EachEmployeeHasRequiredFields()
    {
        // GIVEN - the humanresources.employee table contains rows

        // WHEN - GET /api/employees is called
        var response = await _client.GetAsync("/api/employees");

        // THEN - each item exposes businessEntityId, jobTitle, and hireDate
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var employees = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
        Assert.NotNull(employees);

        var first = employees.First();
        Assert.True(first.BusinessEntityId > 0);
        Assert.False(string.IsNullOrWhiteSpace(first.JobTitle));
        // HireDate is a DateOnly serialised as "yyyy-MM-dd" — must not be default
        Assert.NotEqual(default, first.HireDate);
    }

    [Fact]
    public async Task Given_EmployeesExist_When_GetAllEmployees_Then_PayFieldsAreIncluded()
    {
        // GIVEN - the humanresources.employee table contains rows with pay denormalisation
        //         columns populated (issue #7, Scenario 6)

        // WHEN - GET /api/employees is called
        var response = await _client.GetAsync("/api/employees");

        // THEN - the DTO exposes currentPayRate, currentPayFrequency, and payChangeDate
        //        (values may be null for rows where pay history was not backfilled)
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var employees = await response.Content.ReadFromJsonAsync<List<EmployeeDto>>();
        Assert.NotNull(employees);

        // The JSON payload must deserialise without error — all pay fields present in shape.
        // At least one employee should have a non-null pay rate in the seeded DB.
        Assert.Contains(employees, e => e.CurrentPayRate.HasValue);
    }

    // Local DTO mirroring the shape the endpoint is expected to return.
    // Self-contained so the test is decoupled from the Application layer DTO
    // (which doesn't exist yet — that's the RED state).
    private record EmployeeDto(
        int BusinessEntityId,
        string JobTitle,
        DateOnly HireDate,
        decimal? CurrentPayRate,
        short? CurrentPayFrequency,
        DateTime? PayChangeDate);
}
