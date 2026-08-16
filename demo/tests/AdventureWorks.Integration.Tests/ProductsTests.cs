using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AdventureWorks.Integration.Tests;

/// <summary>
/// Integration tests for GET /api/products — issue #17.
/// Uses WebApplicationFactory to spin up the full ASP.NET Core pipeline in-process.
/// </summary>
public class ProductsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Given_ProductsExist_When_GetAllProducts_Then_ReturnsOkWithNonEmptyList()
    {
        // GIVEN - the production.product table contains rows (seeded in the live DB)

        // WHEN - GET /api/products is called
        var response = await _client.GetAsync("/api/products");

        // THEN - 200 OK is returned with a non-empty JSON array
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(products);
        Assert.True(products.Count > 0);
    }

    [Fact]
    public async Task Given_ProductsExist_When_GetAllProducts_Then_EachProductHasRequiredFields()
    {
        // GIVEN - the production.product table contains rows

        // WHEN - GET /api/products is called
        var response = await _client.GetAsync("/api/products");

        // THEN - each item in the list exposes productId, name, productNumber, listPrice
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content.ReadFromJsonAsync<List<ProductDto>>();
        Assert.NotNull(products);

        var first = products.First();
        Assert.True(first.ProductId > 0);
        Assert.False(string.IsNullOrWhiteSpace(first.Name));
        Assert.False(string.IsNullOrWhiteSpace(first.ProductNumber));
        Assert.True(first.ListPrice >= 0);
    }

    // Local DTO mirroring the shape the endpoint is expected to return.
    // Defined here so the test file is self-contained and decoupled from the
    // Application layer DTO (which doesn't exist yet — that's the RED state).
    private record ProductDto(int ProductId, string Name, string ProductNumber, decimal ListPrice);
}
