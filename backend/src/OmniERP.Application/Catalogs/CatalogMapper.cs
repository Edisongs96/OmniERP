using OmniERP.Application.Catalogs.Dtos;
using OmniERP.Domain.Catalogs;

namespace OmniERP.Application.Catalogs;

public static class CatalogMapper
{
    public static CatalogItemResponse ToResponse(CatalogItem item)
    {
        return new CatalogItemResponse
        {
            Id = item.Id,
            Name = item.Name
        };
    }
}
