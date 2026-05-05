using OmniERP.Domain.Catalogs;

namespace OmniERP.Application.Ports;

public interface ICatalogRepository
{
    Task<IReadOnlyCollection<CatalogItem>> GetOrderStatusesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<CatalogItem>> GetShippingMethodsAsync(CancellationToken cancellationToken = default);
}
