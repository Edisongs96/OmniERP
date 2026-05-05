using OmniERP.Application.Ports;
using OmniERP.Domain.Catalogs;
using OmniERP.Infrastructure.Simulators;

namespace OmniERP.Infrastructure.Repositories;

public sealed class CatalogRepository : ICatalogRepository
{
    private readonly SlowCatalogSource _catalogSource;

    public CatalogRepository(SlowCatalogSource catalogSource)
    {
        _catalogSource = catalogSource;
    }

    public Task<IReadOnlyCollection<CatalogItem>> GetOrderStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        return _catalogSource.GetOrderStatusesAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<CatalogItem>> GetShippingMethodsAsync(
        CancellationToken cancellationToken = default)
    {
        return _catalogSource.GetShippingMethodsAsync(cancellationToken);
    }
}
