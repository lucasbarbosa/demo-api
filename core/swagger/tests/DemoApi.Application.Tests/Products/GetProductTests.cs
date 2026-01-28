namespace DemoApi.Application.Tests.Products;

using DemoApi.Application.Models.Products;
using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
using DemoApi.Tests.Builders.Products;
using FluentAssertions;
using Moq;
public class GetProductTests : ProductTests
{
    [Fact]
    public async Task GetAll_ShouldReturnAllProducts_WhenRepositoryHasProducts()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        List<Product> productsFake =
        [
            ProductBuilder.New().Build(),
            ProductBuilder.New().Build(),
            ProductBuilder.New().Build()
        ];

        productRepository
            .Setup(x => x.GetAll())
            .ReturnsAsync(productsFake);


        // Act
        IList<ProductViewModel> result = await productApplication.GetAll();


        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllBeOfType<ProductViewModel>();

        productRepository.Verify(
            x => x.GetAll(),
            Times.Once
        );
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenRepositoryHasNoProducts()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        List<Product> productsFake = [];

        productRepository
            .Setup(x => x.GetAll())
            .ReturnsAsync(productsFake);


        // Act
        IList<ProductViewModel> result = await productApplication.GetAll();


        // Assert
        result.Should().NotBeNull()
            .And.BeEmpty();

        productRepository.Verify(
            x => x.GetAll(),
            Times.Once
        );
    }

    [Fact]
    public async Task GetById_ShouldReturnProduct_WhenProductExists()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        Product productFake = ProductBuilder.New().Build();

        productRepository
            .Setup(x => x.GetById(productFake.Id))
            .ReturnsAsync(productFake);


        // Act
        ProductViewModel? result = await productApplication.GetById(productFake.Id);


        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(productFake.Name);
        result.Weight.Should().Be(productFake.Weight);

        productRepository.Verify(
            x => x.GetById(productFake.Id),
            Times.Once
        );

        notificator.Verify(
            x => x.AddError(It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        Product nonExistentProduct = ProductBuilder.New().WithId(99999).Build();

        productRepository
            .Setup(x => x.GetById(nonExistentProduct.Id))
            .ReturnsAsync((Product?)null);


        // Act
        ProductViewModel? result = await productApplication.GetById(nonExistentProduct.Id);


        // Assert
        result.Should().BeNull();

        productRepository.Verify(
            x => x.GetById(nonExistentProduct.Id),
            Times.Once
        );

        notificator.Verify(
            x => x.AddError("Product was not found"),
            Times.Once
        );
    }
}