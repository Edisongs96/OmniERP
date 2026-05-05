using Microsoft.Extensions.DependencyInjection;
using OmniERP.Application.Catalogs.UseCases;
using OmniERP.Application.Orders.UseCases;

namespace OmniERP.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetOrderByIdUseCase>();
        services.AddScoped<UpdateOrderUseCase>();
        services.AddScoped<GetOrderCatalogsUseCase>();

        return services;
    }
}
