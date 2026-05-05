using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using OmniERP.Application.Catalogs.Dtos;
using OmniERP.Application.Common;
using OmniERP.Application.Orders.Dtos;

namespace OmniERP.Api.Tests;

public sealed class ApiEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Health_ShouldReturnHealthy()
    {
        await using var factory = new ApiTestApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");
        var body = await response.Content.ReadFromJsonAsync<HealthResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Healthy", body.Status);
        Assert.Equal("OmniERP Orders API", body.Service);
    }

    [Fact]
    public async Task GetOrderById_WithExistingOrder_ShouldReturnOk()
    {
        await using var factory = new ApiTestApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/orders/1001");
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(order);
        Assert.Equal(1001, order.Id);
        Assert.Equal(1, order.Version);
        Assert.Single(order.Items);
    }

    [Fact]
    public async Task GetOrderById_WithMissingOrder_ShouldReturnNotFound()
    {
        await using var factory = new ApiTestApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/orders/9999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetCatalogs_FirstCall_ShouldReturnSlowSourceMetadata()
    {
        await using var factory = new ApiTestApplicationFactory();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/catalogs/order-form");
        var catalogs = await response.Content.ReadFromJsonAsync<OrderFormCatalogsResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(catalogs);
        Assert.Equal("slow-source", catalogs.Metadata.Source);
        Assert.Equal(CacheKeys.OrderFormCatalogs, catalogs.Metadata.CacheKey);
        Assert.NotEmpty(catalogs.OrderStatuses);
        Assert.NotEmpty(catalogs.ShippingMethods);
    }

    [Fact]
    public async Task GetCatalogs_SecondCall_ShouldReturnCacheMetadata()
    {
        await using var factory = new ApiTestApplicationFactory();
        var client = factory.CreateClient();

        await client.GetAsync("/api/v1/catalogs/order-form");
        var response = await client.GetAsync("/api/v1/catalogs/order-form");
        var catalogs = await response.Content.ReadFromJsonAsync<OrderFormCatalogsResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(catalogs);
        Assert.Equal("cache", catalogs.Metadata.Source);
        Assert.Equal(CacheKeys.OrderFormCatalogs, catalogs.Metadata.CacheKey);
    }

    [Fact]
    public async Task UpdateOrder_WithValidVersion_ShouldReturnOkAndIncrementVersion()
    {
        await using var factory = new ApiTestApplicationFactory();
        var client = factory.CreateClient();
        var request = CreateUpdateRequest(version: 1);

        var response = await client.PutAsJsonAsync("/api/v1/orders/1001", request, JsonOptions);
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(order);
        Assert.Equal(2, order.Version);
        Assert.Equal("Nueva direccion guardada por API test", order.DeliveryAddress);
    }

    [Fact]
    public async Task UpdateOrder_WithStaleVersion_ShouldReturnConflict()
    {
        await using var factory = new ApiTestApplicationFactory();
        var client = factory.CreateClient();

        await client.PutAsJsonAsync("/api/v1/orders/1001", CreateUpdateRequest(version: 1), JsonOptions);

        var response = await client.PutAsJsonAsync(
            "/api/v1/orders/1001",
            CreateUpdateRequest(version: 1, internalComment: "Intento con version antigua"),
            JsonOptions);

        var conflict = await response.Content.ReadFromJsonAsync<OrderConflictResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(conflict);
        Assert.Equal(ErrorCodes.OrderConcurrencyConflict, conflict.Code);
        Assert.Equal(2, conflict.CurrentOrder.Version);
        Assert.Equal(1, conflict.AttemptedChanges.Version);
    }

    private static UpdateOrderRequest CreateUpdateRequest(int version, string internalComment = "Actualizado desde API test")
    {
        return new UpdateOrderRequest
        {
            CustomerName = "Cliente Demo",
            CustomerEmail = "cliente.demo@omnierp.local",
            DeliveryAddress = "Nueva direccion guardada por API test",
            InternalComment = internalComment,
            StatusId = 2,
            ShippingMethodId = 3,
            Version = version,
            UpdatedBy = "api.test"
        };
    }

    private sealed record HealthResponse(string Status, string Service);
}
