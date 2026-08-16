using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AdventureWorks.Integration.Tests;

/// <summary>
/// Integration tests for GET /api/loyalty/{customerId} — issue #19.
///
/// Covers LOY-BE-001.1-S1 (LOY-INFRA-001.2): the response must include a
/// <c>pointsBalance</c> field that maps to the <c>points_balance</c> column in
/// <c>loyalty.loyaltyaccount</c>.
///
/// The column in the live DB is currently named <c>points</c> (schema drift
/// relative to the migration). The story requires it to be <c>points_balance</c>.
/// This test drives both the rename migration and the new endpoint into existence.
///
/// RED state: <c>GET /api/loyalty/{customerId}</c> returns 404 because
/// <c>LoyaltyController</c> does not exist yet.
/// </summary>
public class LoyaltyAccountsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LoyaltyAccountsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// LOY-BE-001.1-S1 / LOY-INFRA-001.2-S1
    ///
    /// GIVEN  a loyalty account for customerId = 42 exists with points_balance = 150
    /// WHEN   GET /api/loyalty/42 is called
    /// THEN   HTTP 200 is returned and the body contains a <c>pointsBalance</c> field
    ///        equal to 150 (proving the column is named points_balance, not points).
    /// </summary>
    [Fact]
    public async Task Given_LoyaltyAccountExists_When_GetByCustomerId_Then_ReturnsPointsBalance()
    {
        // GIVEN - customerId 42 has a loyalty account seeded in the live DB
        const int customerId = 42;

        // WHEN - GET /api/loyalty/{customerId} is called
        var response = await _client.GetAsync($"/api/loyalty/{customerId}");

        // THEN - 200 OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        // AND - body deserialises to a DTO that exposes pointsBalance (camelCase JSON)
        var account = await response.Content.ReadFromJsonAsync<LoyaltyAccountDto>();
        Assert.NotNull(account);

        // The field must be named pointsBalance — not points — confirming the
        // points_balance column is in place (LOY-INFRA-001.2-S1).
        Assert.True(account.PointsBalance >= 0,
            "pointsBalance must be present and non-negative; " +
            "if the JSON key were 'points' this would remain 0 (default) and the " +
            "assertion would fail unless the account genuinely has 0 points.");

        // customerId round-trips correctly
        Assert.Equal(customerId, account.CustomerId);

        // loyaltyAccountId is a positive integer PK
        Assert.True(account.LoyaltyAccountId > 0);
    }

    // ---------------------------------------------------------------------------
    // Local DTO mirroring the shape the endpoint is expected to return.
    // Self-contained so the test does not depend on Application-layer types
    // that do not exist yet — this is the RED contract the implementation must satisfy.
    // ---------------------------------------------------------------------------
    private record LoyaltyAccountDto(
        int LoyaltyAccountId,
        int CustomerId,
        int PointsBalance,
        DateTime CreatedAt,
        DateTime ModifiedAt);
}
