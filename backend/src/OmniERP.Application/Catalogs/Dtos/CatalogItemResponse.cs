namespace OmniERP.Application.Catalogs.Dtos;

public sealed record CatalogItemResponse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}
