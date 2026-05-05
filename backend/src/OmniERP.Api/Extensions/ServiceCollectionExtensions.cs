using Microsoft.OpenApi.Models;
using OmniERP.Application;
using OmniERP.Infrastructure;

namespace OmniERP.Api.Extensions;

public static class ServiceCollectionExtensions
{
    private const string FrontendCorsPolicy = "FrontendLocalhost";

    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "OmniERP Orders API",
                Version = "v1",
                Description = "Proof of Concept for OmniERP Order Editing Module"
            });
        });

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendCorsPolicy, policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddApplication();
        services.AddInfrastructure();

        return services;
    }

    public static IApplicationBuilder UseFrontendCors(this IApplicationBuilder app)
    {
        return app.UseCors(FrontendCorsPolicy);
    }
}
