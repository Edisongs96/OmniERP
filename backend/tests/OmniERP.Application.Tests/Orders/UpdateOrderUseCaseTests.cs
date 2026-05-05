using OmniERP.Application.Common;
using OmniERP.Application.Orders.Dtos;
using OmniERP.Application.Orders.UseCases;
using OmniERP.Application.Tests.Fakes;

namespace OmniERP.Application.Tests.Orders;

public sealed class UpdateOrderUseCaseTests
{
    [Fact]
    public async Task UpdateOrder_WithValidVersion_ShouldReturnSuccessAndIncrementVersion()
    {
        var repository = new FakeOrderRepository();
        repository.Add(GetOrderByIdUseCaseTests.CreateOrder(version: 2));
        var useCase = new UpdateOrderUseCase(repository);

        var result = await useCase.ExecuteAsync(1001, CreateRequest(version: 2));

        Assert.Equal(UpdateOrderResultStatus.Success, result.Status);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Order);
        Assert.Equal(3, result.Order.Version);
        Assert.Equal("Updated customer", result.Order.CustomerName);
        Assert.Equal(1, repository.SaveCount);
    }

    [Fact]
    public async Task UpdateOrder_WithStaleVersion_ShouldReturnConflictAndNotSave()
    {
        var repository = new FakeOrderRepository();
        repository.Add(GetOrderByIdUseCaseTests.CreateOrder(version: 5));
        var useCase = new UpdateOrderUseCase(repository);

        var result = await useCase.ExecuteAsync(1001, CreateRequest(version: 4));

        Assert.Equal(UpdateOrderResultStatus.Conflict, result.Status);
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Conflict);
        Assert.Equal(ErrorCodes.OrderConcurrencyConflict, result.Conflict.Code);
        Assert.Equal(5, result.Conflict.CurrentOrder.Version);
        Assert.Equal(4, result.Conflict.AttemptedChanges.Version);
        Assert.Equal(0, repository.SaveCount);
    }

    [Fact]
    public async Task UpdateOrder_WithMissingOrder_ShouldReturnNotFound()
    {
        var useCase = new UpdateOrderUseCase(new FakeOrderRepository());

        var result = await useCase.ExecuteAsync(9999, CreateRequest(version: 1));

        Assert.Equal(UpdateOrderResultStatus.NotFound, result.Status);
        Assert.False(result.IsSuccess);
        Assert.Null(result.Order);
        Assert.Null(result.Conflict);
    }

    [Fact]
    public async Task UpdateOrder_WithInvalidRequest_ShouldReturnValidationError()
    {
        var useCase = new UpdateOrderUseCase(new FakeOrderRepository());

        var result = await useCase.ExecuteAsync(1001, null!);

        Assert.Equal(UpdateOrderResultStatus.ValidationError, result.Status);
        Assert.False(result.IsSuccess);
    }

    private static UpdateOrderRequest CreateRequest(int version)
    {
        return new UpdateOrderRequest
        {
            CustomerName = "Updated customer",
            CustomerEmail = "updated@acme.test",
            DeliveryAddress = "Updated delivery address",
            InternalComment = "Updated internal comment",
            StatusId = 2,
            ShippingMethodId = 3,
            Version = version,
            UpdatedBy = "agent.one"
        };
    }
}
