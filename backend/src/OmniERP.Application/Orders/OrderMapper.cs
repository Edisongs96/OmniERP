using OmniERP.Application.Orders.Dtos;
using OmniERP.Domain.Orders;

namespace OmniERP.Application.Orders;

public static class OrderMapper
{
    public static OrderResponse ToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            DeliveryAddress = order.DeliveryAddress,
            InternalComment = order.InternalComment,
            StatusId = order.StatusId,
            ShippingMethodId = order.ShippingMethodId,
            Version = order.Version,
            UpdatedAt = order.UpdatedAt,
            UpdatedBy = order.UpdatedBy,
            Items = order.Items.Select(ToResponse).ToArray()
        };
    }

    private static OrderItemResponse ToResponse(OrderItem item)
    {
        return new OrderItemResponse
        {
            Id = item.Id,
            OrderId = item.OrderId,
            ProductSku = item.ProductSku,
            ProductName = item.ProductName,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        };
    }
}
