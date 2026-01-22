using DemoApi.Application.Interfaces;
using DemoApi.Application.Services;
using DemoApi.Application.Validators.Products;
using DemoApi.Domain.Handlers;
using DemoApi.Domain.Interfaces;
using DemoApi.Infra.CrossCutting.Logging;
using DemoApi.Infra.Data.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using ILogger = DemoApi.Infra.CrossCutting.Interfaces.ILogger;

namespace DemoApi.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        #region Public Methods

        public static IServiceCollection AddDependencyInjectionConfig(this IServiceCollection services)
        {
            #region Applications

            services.AddScoped<IProductAppService, ProductAppService>();

            #endregion

            #region Repositories

            services.AddScoped<IProductRepository, ProductRepository>();

            #endregion

            #region FluentValidation

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<ProductValidator>();

            #endregion

            #region Others

            services.AddScoped<ILogger, NLogLogger>();
            services.AddScoped<INotificatorHandler, NotificatorHandler>();

            #endregion

            return services;
        }

        #endregion
    }
}