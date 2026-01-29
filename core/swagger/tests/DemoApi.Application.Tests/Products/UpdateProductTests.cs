using DemoApi.Application.Models.Products;
using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
using DemoApi.Tests.Builders.Products;

using FluentAssertions;

using Moq;

namespace DemoApi.Application.Tests.Products;

public class UpdateProductTests : ProductTests
{
    [Fact]
    public async Task Update_ShouldReturnTrue_WhenRepositoryUpdatesSuccessfully()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        Product productFake = ProductBuilder.New().Build();
        ProductViewModel productViewModel = _mapper.Map<ProductViewModel>(productFake);

        productRepository
            .Setup(x => x.GetById(productViewModel.Id))
            .ReturnsAsync(productFake);

        productRepository
            .Setup(x => x.Update(It.IsAny<Product>()))
            .ReturnsAsync(true);


        // Act
        bool result = await productApplication.Update(productViewModel);


        // Assert
        result.Should().BeTrue();

        productRepository.Verify(
            x => x.GetById(productViewModel.Id),
            Times.Once
        );

        productRepository.Verify(
            x => x.Update(It.IsAny<Product>()),
            Times.Once
        );

        notificator.Verify(
            x => x.AddError(It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task Update_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        ProductViewModel productViewModel = ProductViewModelBuilder.New().Build();

        productRepository
            .Setup(x => x.GetById(productViewModel.Id))
            .ReturnsAsync((Product?)null);


        // Act
        bool result = await productApplication.Update(productViewModel);


        // Assert
        result.Should().BeFalse();

        productRepository.Verify(
            x => x.GetById(productViewModel.Id),
            Times.Once
        );

        productRepository.Verify(
            x => x.Update(It.IsAny<Product>()),
            Times.Never
        );

        notificator.Verify(
            x => x.AddError("Product was not found"),
            Times.Once
        );
    }

    [Fact]
    public async Task Update_ShouldReturnFalse_WhenProductIsNull()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        ProductViewModel? nullProduct = null;


        // Act
        bool result = await productApplication.Update(nullProduct!);


        // Assert
        result.Should().BeFalse();

        productRepository.Verify(
            x => x.GetById(It.IsAny<uint>()),
            Times.Never
        );

        productRepository.Verify(
            x => x.Update(It.IsAny<Product>()),
            Times.Never
        );

        notificator.Verify(
            x => x.AddError("Product could not be updated"),
            Times.Once
        );
    }

    [Fact]
    public async Task Update_ShouldReturnFalse_WhenRepositoryUpdateFails()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        Product productFake = ProductBuilder.New().Build();
        ProductViewModel productViewModel = _mapper.Map<ProductViewModel>(productFake);

        productRepository
            .Setup(x => x.GetById(productViewModel.Id))
            .ReturnsAsync(productFake);

        productRepository
            .Setup(x => x.Update(It.IsAny<Product>()))
            .ReturnsAsync(false);


        // Act
        bool result = await productApplication.Update(productViewModel);


        // Assert
        result.Should().BeFalse();

        productRepository.Verify(
            x => x.GetById(productViewModel.Id),
            Times.Once
        );

        productRepository.Verify(
            x => x.Update(It.IsAny<Product>()),
            Times.Once
        );

        notificator.Verify(
            x => x.AddError("Product could not be updated"),
            Times.Once
        );
    }
}