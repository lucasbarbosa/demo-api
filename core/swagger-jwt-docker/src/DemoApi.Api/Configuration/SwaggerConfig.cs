using Microsoft.OpenApi;

namespace DemoApi.Api.Configuration;

public static class SwaggerConfig
{
    #region Public Methods

    public static IServiceCollection AddSwaggerConfig(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Demo API",
                Version = "v1",
                Description = "RESTful API built with .NET 10 demonstrating Clean Architecture, SOLID principles, and enterprise-grade design patterns. Features comprehensive testing, type-safe responses, JWT authentication, production-ready error handling, and full Docker support for easy deployment and scalability.",
                Contact = new OpenApiContact
                {
                    Name = "Lucas Barbosa",
                    Url = new Uri("https://github.com/lucasbarbosa/demo-api")
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", doc),
                    new List<string>()
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.DocumentTitle = "Demo API - Documentation";
            options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
            options.DefaultModelsExpandDepth(-1);
        });

        return app;
    }

    #endregion
}