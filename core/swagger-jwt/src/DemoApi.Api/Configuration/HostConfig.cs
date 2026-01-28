namespace DemoApi.Api.Configuration
{
    public static class HostConfig
    {
        public static WebApplicationBuilder AddHostConfig(this WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

            return builder;
        }
    }
}