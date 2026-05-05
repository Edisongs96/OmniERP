using OmniERP.Api.Extensions;
using OmniERP.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();

var app = builder.Build();

app.UseApiExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseFrontendCors();
app.MapControllers();

await SeedDatabaseAsync(app);

await app.RunAsync();

static async Task SeedDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();

    await seeder.SeedAsync();
}

public partial class Program;
