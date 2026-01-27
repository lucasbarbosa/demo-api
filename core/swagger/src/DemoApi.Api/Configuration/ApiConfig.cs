using Asp.Versioning;
using DemoApi.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Api.Configuration
{
    public static class ApiConfig
    {
        #region Public Methods

        public static IServiceCollection AddApiConfig(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add<ModelValidationFilter>();
                options.Filters.Add<FluentValidationFilter>();
            });

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"),
                    new QueryStringApiVersionReader("api-version")
                );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

            //Custom ModelState Messages
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerConfig();

            return services;
        }

        public static IApplicationBuilder UseApiConfig(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseSwaggerConfig();

            return app;
        }

        #endregion
    }
}