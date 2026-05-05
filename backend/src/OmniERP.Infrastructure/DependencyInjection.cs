using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OmniERP.Application.Ports;
using OmniERP.Infrastructure.Cache;
using OmniERP.Infrastructure.Persistence;
using OmniERP.Infrastructure.Repositories;
using OmniERP.Infrastructure.Seed;
using OmniERP.Infrastructure.Simulators;

namespace OmniERP.Infrastructure;

public static class DependencyInjection
{
    private const string DatabaseName = "OmniERP-Orders-PoC";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<OmniErpDbContext>(options =>
            options.UseInMemoryDatabase(DatabaseName));

        services.AddMemoryCache();

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICatalogRepository, CatalogRepository>();
        services.AddScoped<ICacheProvider, MemoryCacheProvider>();
        services.AddScoped<DatabaseSeeder>();
        services.AddSingleton<SlowCatalogSource>();

        return services;
    }
}
