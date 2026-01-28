using DemoApi.Api.Configuration;
using DemoApi.Application.Automapper;
using NLog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

Logger logger = builder.AddNLogConfig();

try
{
    builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(AutomapperConfig).Assembly));

    builder.Services.AddDependencyInjectionConfig();

    builder.Services.AddApiConfig();

    WebApplication app = builder.Build();

    app.UseApiConfig(app.Environment);

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLogConfig.Shutdown();
}

public partial class Program { }