using OmniERP.Domain.Common;

namespace OmniERP.Domain.Orders;

public sealed class Order : Entity
{
    private readonly List<OrderItem> _items;

    private Order()
        : base(0)
    {
        CustomerName = string.Empty;
        CustomerEmail = string.Empty;
        DeliveryAddress = string.Empty;
        InternalComment = string.Empty;
        UpdatedBy = string.Empty;
        _items = [];
    }

    public Order(
        int id,
        string customerName,
        string customerEmail,
        string deliveryAddress,
        string? internalComment,
        int statusId,
        int shippingMethodId,
        int version,
        DateTime updatedAt,
        string updatedBy,
        IEnumerable<OrderItem>? items = null)
        : base(id)
    {
        CustomerName = Required(customerName, nameof(customerName));
        CustomerEmail = Required(customerEmail, nameof(customerEmail));
        DeliveryAddress = Required(deliveryAddress, nameof(deliveryAddress));
        InternalComment = internalComment?.Trim() ?? string.Empty;
        StatusId = statusId;
        ShippingMethodId = shippingMethodId;
        Version = version;
        UpdatedAt = updatedAt;
        UpdatedBy = Required(updatedBy, nameof(updatedBy));
        _items = items?.ToList() ?? [];
    }

    public string CustomerName { get; private set; }

    public string CustomerEmail { get; private set; }

    public string DeliveryAddress { get; private set; }

    public string InternalComment { get; private set; }

    public int StatusId { get; private set; }

    public int ShippingMethodId { get; private set; }

    public int Version { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public string UpdatedBy { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public void Update(
        string customerName,
        string customerEmail,
        string deliveryAddress,
        string? internalComment,
        int statusId,
        int shippingMethodId,
        int expectedVersion,
        string updatedBy)
    {
        if (expectedVersion != Version)
        {
            throw new OrderConflictException(Id, Version, expectedVersion);
        }

        var normalizedCustomerName = Required(customerName, nameof(customerName));
        var normalizedCustomerEmail = Required(customerEmail, nameof(customerEmail));
        var normalizedDeliveryAddress = Required(deliveryAddress, nameof(deliveryAddress));
        var normalizedInternalComment = internalComment?.Trim() ?? string.Empty;
        var normalizedUpdatedBy = Required(updatedBy, nameof(updatedBy));

        CustomerName = normalizedCustomerName;
        CustomerEmail = normalizedCustomerEmail;
        DeliveryAddress = normalizedDeliveryAddress;
        InternalComment = normalizedInternalComment;
        StatusId = statusId;
        ShippingMethodId = shippingMethodId;
        Version += 1;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = normalizedUpdatedBy;
    }

    private static string Required(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("El valor es requerido.", parameterName);
        }

        return value.Trim();
    }
}
