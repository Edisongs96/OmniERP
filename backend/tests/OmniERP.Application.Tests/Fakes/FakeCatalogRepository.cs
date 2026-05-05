using OmniERP.Application.Ports;
using OmniERP.Domain.Catalogs;

namespace OmniERP.Application.Tests.Fakes;

internal sealed class FakeCatalogRepository : ICatalogRepository
{
    private readonly IReadOnlyCollection<CatalogItem> _orderStatuses;
    private readonly IReadOnlyCollection<CatalogItem> _shippingMethods;

    public FakeCatalogRepository(
        IReadOnlyCollection<CatalogItem>? orderStatuses = null,
        IReadOnlyCollection<CatalogItem>? shippingMethods = null)
    {
        _orderStatuses = orderStatuses ?? new[]
        {
            new CatalogItem(1, "Draft"),
            new CatalogItem(2, "Confirmed")
        };

        _shippingMethods = shippingMethods ?? new[]
        {
            new CatalogItem(1, "Standard"),
            new CatalogItem(2, "Express")
        };
    }

    public int OrderStatusesCallCount { get; private set; }

    public int ShippingMethodsCallCount { get; private set; }

    public Task<IReadOnlyCollection<CatalogItem>> GetOrderStatusesAsync(CancellationToken cancellationToken = default)
    {
        OrderStatusesCallCount += 1;

        return Task.FromResult(_orderStatuses);
    }

    public Task<IReadOnlyCollection<CatalogItem>> GetShippingMethodsAsync(CancellationToken cancellationToken = default)
    {
        ShippingMethodsCallCount += 1;

        return Task.FromResult(_shippingMethods);
    }
}
