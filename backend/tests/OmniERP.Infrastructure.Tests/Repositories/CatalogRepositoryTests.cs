using System.Diagnostics;
using OmniERP.Infrastructure.Repositories;
using OmniERP.Infrastructure.Simulators;

namespace OmniERP.Infrastructure.Tests.Repositories;

public sealed class CatalogRepositoryTests
{
    [Fact]
    public async Task CatalogRepository_GetOrderStatusesAsync_ShouldSimulateSlowSource()
    {
        var repository = new CatalogRepository(new SlowCatalogSource());
        var stopwatch = Stopwatch.StartNew();

        var statuses = await repository.GetOrderStatusesAsync();

        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds >= 1900);
        Assert.Contains(statuses, status => status.Name == "Pendiente");
    }

    [Fact]
    public async Task CatalogRepository_GetShippingMethodsAsync_ShouldReturnExpectedItems()
    {
        var repository = new CatalogRepository(new SlowCatalogSource());

        var shippingMethods = await repository.GetShippingMethodsAsync();

        Assert.Contains(shippingMethods, method => method.Name == "Recogida en tienda");
        Assert.Contains(shippingMethods, method => method.Name == "Transportadora aliada");
        Assert.Equal(4, shippingMethods.Count);
    }
}
