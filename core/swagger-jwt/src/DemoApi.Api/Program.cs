using DemoApi.Api.Configuration;
using DemoApi.Application.Automapper;

using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddHostConfig();

builder.AddSerilogConfiguration();

try
{
    builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AutomapperConfig).Assembly));

    builder.Services.AddDependencyInjectionConfig();

    builder.Services.AddJwtConfig(builder.Configuration);

    builder.Services.AddApiConfig();

    WebApplication app = builder.Build();

    app.UseApiConfig(app.Environment);

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Stopped program because of exception");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program { }