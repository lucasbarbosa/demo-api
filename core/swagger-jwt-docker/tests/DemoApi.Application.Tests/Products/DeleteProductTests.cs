using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
using DemoApi.Tests.Builders.Products;

using FluentAssertions;

using Moq;

namespace DemoApi.Application.Tests.Products;

public class DeleteProductTests : ProductTests
{
    [Fact]
    public async Task DeleteById_ShouldReturnTrue_WhenRepositoryDeletesSuccessfully()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        Product productFake = ProductBuilder.New().Build();

        productRepository
            .Setup(x => x.GetById(productFake.Id))
            .ReturnsAsync(productFake);

        productRepository
            .Setup(x => x.DeleteById(productFake.Id))
            .ReturnsAsync(true);


        // Act
        bool result = await productApplication.DeleteById(productFake.Id);


        // Assert
        result.Should().BeTrue();

        productRepository.Verify(
            x => x.GetById(productFake.Id),
            Times.Once
        );

        productRepository.Verify(
            x => x.DeleteById(productFake.Id),
            Times.Once
        );

        notificator.Verify(
            x => x.AddError(It.IsAny<string>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteById_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        Product nonExistentProduct = ProductBuilder.New().WithId(999999).Build();

        productRepository
            .Setup(x => x.GetById(nonExistentProduct.Id))
            .ReturnsAsync((Product?)null);


        // Act
        bool result = await productApplication.DeleteById(nonExistentProduct.Id);


        // Assert
        result.Should().BeFalse();

        productRepository.Verify(
            x => x.GetById(nonExistentProduct.Id),
            Times.Once
        );

        productRepository.Verify(
            x => x.DeleteById(nonExistentProduct.Id),
            Times.Never
        );

        notificator.Verify(
            x => x.AddError("Product was not found"),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteById_ShouldReturnFalse_WhenRepositoryDeleteFails()
    {
        // Arrange
        (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

        Product productFake = ProductBuilder.New().Build();

        productRepository
            .Setup(x => x.GetById(productFake.Id))
            .ReturnsAsync(productFake);

        productRepository
            .Setup(x => x.DeleteById(productFake.Id))
            .ReturnsAsync(false);


        // Act
        bool result = await productApplication.DeleteById(productFake.Id);


        // Assert
        result.Should().BeFalse();

        productRepository.Verify(
            x => x.GetById(productFake.Id),
            Times.Once
        );

        productRepository.Verify(
            x => x.DeleteById(productFake.Id),
            Times.Once
        );

        notificator.Verify(
            x => x.AddError("Product could not be deleted"),
            Times.Once
        );
    }
}