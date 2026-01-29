using Serilog;

namespace DemoApi.Api.Configuration;

public static class SerilogConfig
{
    public static void AddSerilogConfiguration(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
                         .ReadFrom.Services(services)
                         .Enrich.FromLogContext()
                         .WriteTo.Console());
    }
}
