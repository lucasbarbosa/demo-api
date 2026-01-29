using AutoMapper;

using Bogus;

using DemoApi.Application.Automapper;
using DemoApi.Application.Services;
using DemoApi.Domain.Interfaces;

using Microsoft.Extensions.Logging.Abstractions;

using Moq;

namespace DemoApi.Application.Tests.Products;

public class ProductTests
{
    #region Properties

    protected readonly IMapper _mapper;

    #endregion

    #region Constructors

    public ProductTests()
    {
        MapperConfigurationExpression configExpression = new();
        configExpression.AddMaps(typeof(AutomapperConfig).Assembly);

        MapperConfiguration config = new(configExpression, NullLoggerFactory.Instance);

        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();

        Randomizer.Seed = new Random(1234);
    }

    #endregion

    #region Protected Methods

    protected (Mock<INotificatorHandler>, Mock<IProductRepository>, ProductAppService) SetProductAppService()
    {
        Mock<INotificatorHandler> notificator = new();
        Mock<IProductRepository> productRepository = new();
        ProductAppService productApplication = new(
            _mapper,
            notificator.Object,
            productRepository.Object
        );

        return (notificator, productRepository, productApplication);
    }

    #endregion
}