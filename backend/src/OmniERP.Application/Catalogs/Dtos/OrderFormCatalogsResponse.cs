namespace OmniERP.Application.Catalogs.Dtos;

public sealed record OrderFormCatalogsResponse
{
    public IReadOnlyCollection<CatalogItemResponse> OrderStatuses { get; init; } = [];

    public IReadOnlyCollection<CatalogItemResponse> ShippingMethods { get; init; } = [];

    public required CatalogResponseMetadata Metadata { get; init; }
}
