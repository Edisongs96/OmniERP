using OmniERP.Api.Middlewares;

namespace OmniERP.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
