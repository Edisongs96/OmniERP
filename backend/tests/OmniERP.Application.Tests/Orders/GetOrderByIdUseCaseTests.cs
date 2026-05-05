using OmniERP.Application.Orders.UseCases;
using OmniERP.Application.Tests.Fakes;
using OmniERP.Domain.Orders;

namespace OmniERP.Application.Tests.Orders;

public sealed class GetOrderByIdUseCaseTests
{
    [Fact]
    public async Task GetOrderById_WithExistingOrder_ShouldReturnOrder()
    {
        var repository = new FakeOrderRepository();
        repository.Add(CreateOrder(id: 1001));
        var useCase = new GetOrderByIdUseCase(repository);

        var result = await useCase.ExecuteAsync(1001);

        Assert.NotNull(result);
        Assert.Equal(1001, result.Id);
        Assert.Equal("Acme Distribution", result.CustomerName);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetOrderById_WithInvalidId_ShouldThrowOrReturnValidationError()
    {
        var useCase = new GetOrderByIdUseCase(new FakeOrderRepository());

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => useCase.ExecuteAsync(0));
    }

    [Fact]
    public async Task GetOrderById_WithMissingOrder_ShouldReturnNull()
    {
        var useCase = new GetOrderByIdUseCase(new FakeOrderRepository());

        var result = await useCase.ExecuteAsync(9999);

        Assert.Null(result);
    }

    internal static Order CreateOrder(int id = 1001, int version = 1)
    {
        var items = new[]
        {
            new OrderItem(
                id: 1,
                orderId: id,
                productSku: "SKU-001",
                productName: "Demo product",
                quantity: 2,
                unitPrice: 25m)
        };

        return new Order(
            id: id,
            customerName: "Acme Distribution",
            customerEmail: "orders@acme.test",
            deliveryAddress: "Original delivery address",
            internalComment: "Original internal comment",
            statusId: 1,
            shippingMethodId: 2,
            version: version,
            updatedAt: DateTime.UtcNow.AddMinutes(-10),
            updatedBy: "agent.initial",
            items: items);
    }
}
