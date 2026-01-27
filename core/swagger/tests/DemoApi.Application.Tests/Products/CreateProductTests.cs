using DemoApi.Application.Models.Products;
using DemoApi.Application.Services;
using DemoApi.Domain.Entities;
using DemoApi.Domain.Interfaces;
using DemoApi.Tests.Builders.Products;
using FluentAssertions;
using Moq;

namespace DemoApi.Application.Tests.Products
{
    public class CreateProductTests : ProductTests
    {
        [Fact]
        public async Task Create_ShouldReturnProduct_WhenRepositoryCreatesSuccessfully()
        {
            // Arrange
            (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

            Product productFake = ProductBuilder.New().Build();
            ProductViewModel productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.GetByName(productViewModel.Name!))
                .ReturnsAsync((Product?)null);

            productRepository
                .Setup(x => x.Create(It.IsAny<Product>()))
                .ReturnsAsync(productFake);


            // Act
            ProductViewModel? result = await productApplication.Create(productViewModel);


            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be(productFake.Name);
            result.Weight.Should().Be(productFake.Weight);

            productRepository.Verify(
                x => x.GetByName(productViewModel.Name!),
                Times.Once
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError(It.IsAny<string>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenProductAlreadyExists()
        {
            // Arrange
            (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

            Product productFake = ProductBuilder.New().Build();
            ProductViewModel productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.GetByName(productViewModel.Name!))
                .ReturnsAsync(productFake);


            // Act
            ProductViewModel? result = await productApplication.Create(productViewModel);


            // Assert
            result.Should().BeNull();

            productRepository.Verify(
                x => x.GetByName(productViewModel.Name!),
                Times.Once
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError($"Product ({productViewModel.Name}) is already registered"),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
        {
            // Arrange
            (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

            Product productFake = ProductBuilder.New().Build();
            ProductViewModel productViewModel = _mapper.Map<ProductViewModel>(productFake);

            productRepository
                .Setup(x => x.GetByName(productViewModel.Name!))
                .ReturnsAsync((Product?)null);

            productRepository.Setup(x => x.Create(It.IsAny<Product>())).ReturnsAsync(default(Product)!);


            // Act
            ProductViewModel? result = await productApplication.Create(productViewModel);


            // Assert
            result.Should().BeNull();

            productRepository.Verify(
                x => x.GetByName(productViewModel.Name!),
                Times.Once
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Once
            );

            notificator.Verify(
                x => x.AddError("Product could not be created"),
                Times.Once
            );
        }

        [Fact]
        public async Task Create_ShouldReturnNull_WhenProductIsNull()
        {
            // Arrange
            (Mock<INotificatorHandler> notificator, Mock<IProductRepository> productRepository, ProductAppService productApplication) = SetProductAppService();

            ProductViewModel? nullProduct = null;


            // Act
            ProductViewModel? result = await productApplication.Create(nullProduct!);


            // Assert
            result.Should().BeNull();

            productRepository.Verify(
                x => x.GetByName(It.IsAny<string>()),
                Times.Never
            );

            productRepository.Verify(
                x => x.Create(It.IsAny<Product>()),
                Times.Never
            );

            notificator.Verify(
                x => x.AddError("Product could not be created"),
                Times.Once
            );
        }
    }
}