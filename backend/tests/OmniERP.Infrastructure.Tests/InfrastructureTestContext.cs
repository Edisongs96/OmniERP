using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OmniERP.Infrastructure.Persistence;

namespace OmniERP.Infrastructure.Tests;

internal static class InfrastructureTestContext
{
    public static OmniErpDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<OmniErpDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new OmniErpDbContext(options);
    }

    public static OmniErpDbContext CreateDbContext(string databaseName, InMemoryDatabaseRoot databaseRoot)
    {
        var options = new DbContextOptionsBuilder<OmniErpDbContext>()
            .UseInMemoryDatabase(databaseName, databaseRoot)
            .Options;

        return new OmniErpDbContext(options);
    }
}
