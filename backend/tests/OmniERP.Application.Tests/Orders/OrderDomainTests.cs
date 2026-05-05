using OmniERP.Domain.Orders;

namespace OmniERP.Application.Tests.Orders;

public sealed class OrderDomainTests
{
    [Fact]
    public void Order_Update_WithValidVersion_ShouldUpdateFieldsAndIncrementVersion()
    {
        var originalUpdatedAt = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var order = CreateOrder(version: 3, updatedAt: originalUpdatedAt);

        order.Update(
            customerName: "Acme Updated",
            customerEmail: "updated@acme.test",
            deliveryAddress: "New delivery address",
            internalComment: "New internal comment",
            statusId: 2,
            shippingMethodId: 4,
            expectedVersion: 3,
            updatedBy: "agent.one");

        Assert.Equal("Acme Updated", order.CustomerName);
        Assert.Equal("updated@acme.test", order.CustomerEmail);
        Assert.Equal("New delivery address", order.DeliveryAddress);
        Assert.Equal("New internal comment", order.InternalComment);
        Assert.Equal(2, order.StatusId);
        Assert.Equal(4, order.ShippingMethodId);
        Assert.Equal(4, order.Version);
        Assert.Equal("agent.one", order.UpdatedBy);
        Assert.True(order.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void Order_Update_WithStaleVersion_ShouldThrowOrderConflictException()
    {
        var order = CreateOrder(version: 5);

        var exception = Assert.Throws<OrderConflictException>(() =>
            order.Update(
                customerName: "Acme Updated",
                customerEmail: "updated@acme.test",
                deliveryAddress: "New delivery address",
                internalComment: "New internal comment",
                statusId: 2,
                shippingMethodId: 4,
                expectedVersion: 4,
                updatedBy: "agent.two"));

        Assert.Equal(order.Id, exception.OrderId);
        Assert.Equal(5, exception.CurrentVersion);
        Assert.Equal(4, exception.AttemptedVersion);
        Assert.Equal(5, order.Version);
    }

    [Fact]
    public void OrderItem_WithInvalidQuantity_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OrderItem(
                id: 1,
                orderId: 1001,
                productSku: "SKU-001",
                productName: "Demo product",
                quantity: 0,
                unitPrice: 10m));
    }

    [Fact]
    public void OrderItem_WithNegativeUnitPrice_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new OrderItem(
                id: 1,
                orderId: 1001,
                productSku: "SKU-001",
                productName: "Demo product",
                quantity: 1,
                unitPrice: -1m));
    }

    private static Order CreateOrder(int version, DateTime? updatedAt = null)
    {
        var items = new[]
        {
            new OrderItem(
                id: 1,
                orderId: 1001,
                productSku: "SKU-001",
                productName: "Demo product",
                quantity: 2,
                unitPrice: 25m)
        };

        return new Order(
            id: 1001,
            customerName: "Acme Distribution",
            customerEmail: "orders@acme.test",
            deliveryAddress: "Original delivery address",
            internalComment: "Original internal comment",
            statusId: 1,
            shippingMethodId: 2,
            version: version,
            updatedAt: updatedAt ?? DateTime.UtcNow.AddMinutes(-10),
            updatedBy: "agent.initial",
            items: items);
    }
}
