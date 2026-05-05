using OmniERP.Domain.Common;

namespace OmniERP.Domain.Catalogs;

public sealed class CatalogItem : Entity
{
    public CatalogItem(int id, string name)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del catalogo es requerido.", nameof(name));
        }

        Name = name.Trim();
    }

    public string Name { get; }
}
