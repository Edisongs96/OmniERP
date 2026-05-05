namespace OmniERP.Application.Catalogs.Dtos;

public sealed record CatalogResponseMetadata
{
    public string Source { get; init; } = string.Empty;

    public long DurationMs { get; init; }

    public string CacheKey { get; init; } = string.Empty;
}
