namespace OmniERP.Domain.Common;

public abstract class Entity
{
    protected Entity(int id)
    {
        Id = id;
    }

    public int Id { get; protected set; }
}
