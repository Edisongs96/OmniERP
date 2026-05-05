using Microsoft.EntityFrameworkCore;
using OmniERP.Infrastructure.Seed;

namespace OmniERP.Infrastructure.Tests.Seed;

public sealed class DatabaseSeederTests
{
    [Fact]
    public async Task DatabaseSeeder_SeedAsync_ShouldBeIdempotent()
    {
        await using var dbContext = InfrastructureTestContext.CreateDbContext();
        var seeder = new DatabaseSeeder(dbContext);

        await seeder.SeedAsync();
        await seeder.SeedAsync();

        var ordersCount = await dbContext.Orders.CountAsync();
        var orderItemsCount = await dbContext.OrderItems.CountAsync();

        Assert.Equal(1, ordersCount);
        Assert.Equal(1, orderItemsCount);
    }
}
