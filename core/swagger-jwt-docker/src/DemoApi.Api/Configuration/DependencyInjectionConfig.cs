using DemoApi.Application.Services;
using DemoApi.Application.Validators.Products;
using DemoApi.Domain.Handlers;
using DemoApi.Infra.CrossCutting.Interfaces;
using DemoApi.Infra.CrossCutting.Logging;
using DemoApi.Infra.Data.Repositories;

using FluentValidation;

namespace DemoApi.Api.Configuration;

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

        services.AddValidatorsFromAssemblyContaining<ProductValidator>();

        #endregion

        #region Others

        services.AddScoped<DemoApi.Infra.CrossCutting.Interfaces.ILogger, SerilogLogger>();
        services.AddScoped<INotificatorHandler, NotificatorHandler>();

        #endregion

        return services;
    }

    #endregion
}