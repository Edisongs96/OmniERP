using OmniERP.Domain.Catalogs;

namespace OmniERP.Infrastructure.Simulators;

public sealed class SlowCatalogSource
{
    private static readonly IReadOnlyCollection<CatalogItem> OrderStatuses =
    [
        new CatalogItem(1, "Pendiente"),
        new CatalogItem(2, "En preparacion"),
        new CatalogItem(3, "Enviado"),
        new CatalogItem(4, "Entregado"),
        new CatalogItem(5, "Cancelado")
    ];

    private static readonly IReadOnlyCollection<CatalogItem> ShippingMethods =
    [
        new CatalogItem(1, "Recogida en tienda"),
        new CatalogItem(2, "Envio estandar"),
        new CatalogItem(3, "Envio express"),
        new CatalogItem(4, "Transportadora aliada")
    ];

    public async Task<IReadOnlyCollection<CatalogItem>> GetOrderStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        return OrderStatuses;
    }

    public async Task<IReadOnlyCollection<CatalogItem>> GetShippingMethodsAsync(
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        return ShippingMethods;
    }
}
