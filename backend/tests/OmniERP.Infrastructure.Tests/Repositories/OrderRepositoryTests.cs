using Microsoft.EntityFrameworkCore.Storage;
using OmniERP.Infrastructure.Repositories;
using OmniERP.Infrastructure.Seed;

namespace OmniERP.Infrastructure.Tests.Repositories;

public sealed class OrderRepositoryTests
{
    [Fact]
    public async Task OrderRepository_GetByIdAsync_ShouldReturnSeededOrder()
    {
        await using var dbContext = InfrastructureTestContext.CreateDbContext();
        await new DatabaseSeeder(dbContext).SeedAsync();
        var repository = new OrderRepository(dbContext);

        var order = await repository.GetByIdAsync(1001);

        Assert.NotNull(order);
        Assert.Equal("Cliente Demo", order.CustomerName);
        Assert.Single(order.Items);
    }

    [Fact]
    public async Task OrderRepository_SaveAsync_ShouldPersistUpdatedOrder()
    {
        var databaseName = Guid.NewGuid().ToString();
        var databaseRoot = new InMemoryDatabaseRoot();

        await using (var seedContext = InfrastructureTestContext.CreateDbContext(databaseName, databaseRoot))
        {
            await new DatabaseSeeder(seedContext).SeedAsync();
        }

        await using (var updateContext = InfrastructureTestContext.CreateDbContext(databaseName, databaseRoot))
        {
            var repository = new OrderRepository(updateContext);
            var order = await repository.GetByIdAsync(1001);

            Assert.NotNull(order);

            order.Update(
                customerName: "Cliente Demo Actualizado",
                customerEmail: "cliente.demo@omnierp.local",
                deliveryAddress: "Nueva direccion demo",
                internalComment: "Pedido actualizado",
                statusId: 2,
                shippingMethodId: 3,
                expectedVersion: 1,
                updatedBy: "agent.one");

            await repository.SaveAsync(order);
        }

        await using (var assertContext = InfrastructureTestContext.CreateDbContext(databaseName, databaseRoot))
        {
            var repository = new OrderRepository(assertContext);
            var order = await repository.GetByIdAsync(1001);

            Assert.NotNull(order);
            Assert.Equal("Cliente Demo Actualizado", order.CustomerName);
            Assert.Equal(2, order.Version);
            Assert.Equal("agent.one", order.UpdatedBy);
        }
    }
}
