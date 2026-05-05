using OmniERP.Domain.Common;

namespace OmniERP.Domain.Orders;

public sealed class OrderItem : Entity
{
    public OrderItem(
        int id,
        int orderId,
        string productSku,
        string productName,
        int quantity,
        decimal unitPrice)
        : base(id)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad del item debe ser mayor que cero.");
        }

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "El precio unitario del item no puede ser negativo.");
        }

        OrderId = orderId;
        ProductSku = productSku;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public int OrderId { get; }

    public string ProductSku { get; }

    public string ProductName { get; }

    public int Quantity { get; }

    public decimal UnitPrice { get; }
}
