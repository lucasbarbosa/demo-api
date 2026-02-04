using System.Net;
using System.Net.Http.Json;

using DemoApi.Api.Tests.Common.Configuration;
using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Api.Tests.Common.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using DemoApi.Tests.Builders.Products;

using FluentAssertions;

namespace DemoApi.Api.Tests.Products;

public class UpdateProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
{
    #region Public Methods

    [Fact, TestPriority(300)]
    public async Task Update_ShouldReturnNoContent_WhenProductIsValid()
    {
        // Arrange
        string url = "/api/v1/products";

        ProductViewModel createdProduct = await GetLastCreatedProduct();
        ProductViewModel productToUpdate = ProductViewModelBuilder.New()
            .WithId(createdProduct!.Id)
            .WithName("Updated Product Name")
            .WithWeight(5.0)
            .Build();

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(301)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty()
    {
        // Arrange
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New().WithEmptyName().Build();

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        viewModel!.Should().NotBeNull();
        viewModel!.Success.Should().BeFalse();
        viewModel!.Errors.Should().Contain("Name is required");
    }

    [Fact, TestPriority(302)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsNull()
    {
        // Arrange
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New().WithNullName().Build();

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        viewModel!.Should().NotBeNull();
        viewModel!.Success.Should().BeFalse();
        viewModel!.Errors.Should().Contain("Name is required");
    }

    [Fact, TestPriority(303)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsZero()
    {
        // Arrange
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New().WithZeroWeight().Build();

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        viewModel!.Should().NotBeNull();
        viewModel!.Success.Should().BeFalse();
        viewModel!.Errors.Should().Contain("Weight must be greater than 0");
    }

    [Fact, TestPriority(304)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsNegative()
    {
        // Arrange
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New().WithNegativeWeight().Build();

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        viewModel!.Should().NotBeNull();
        viewModel!.Success.Should().BeFalse();
        viewModel!.Errors.Should().Contain("Weight must be greater than 0");
    }

    [Fact, TestPriority(305)]
    public async Task Update_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New().WithNonExistentId().Build();

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeFalse();
    }

    [Fact, TestPriority(306)]
    public async Task Update_ShouldReturnNotFound_WhenProductIdIsZero()
    {
        // Arrange
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New().WithIdZero().Build();

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productFake);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        viewModel.Should().NotBeNull();
        viewModel!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(307)]
    public async Task Update_ShouldAllowChangingWeightOnly()
    {
        // Arrange
        ProductViewModel createdProduct = await GetLastCreatedProduct();

        string url = "/api/v1/products";
        ProductViewModel updatedProduct = ProductViewModelBuilder.New()
            .WithId(createdProduct!.Id)
            .WithName(createdProduct.Name!)
            .WithWeight(createdProduct.Weight + 1.0)
            .Build();

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, updatedProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(308)]
    public async Task Update_ShouldAllowChangingNameOnly()
    {
        // Arrange
        ProductViewModel createdProduct = await GetLastCreatedProduct();
        string url = "/api/v1/products";
        ProductViewModel updatedProduct = ProductViewModelBuilder.New()
            .WithId(createdProduct!.Id)
            .WithName("New Name Only")
            .WithWeight(createdProduct.Weight)
            .Build();

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, updatedProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(309)]
    public async Task Update_ShouldHandleConcurrentUpdates()
    {
        // Arrange
        ProductViewModel createdProduct = await GetLastCreatedProduct();
        string updateUrl = "/api/v1/products";

        List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 3; i++)
        {
            ProductViewModel productToUpdate = ProductViewModelBuilder.New()
                .WithId(createdProduct!.Id)
                .WithName($"Concurrent Update {i}")
                .WithWeight(5.0)
                .Build();
            tasks.Add(_client.PutAsJsonAsync(updateUrl, productToUpdate, TestContext.Current.CancellationToken));
        }

        // Act
        HttpResponseMessage[] results = await Task.WhenAll(tasks);

        // Assert
        int successCount = results.Count(r => r.StatusCode == HttpStatusCode.NoContent);
        successCount.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact, TestPriority(310)]
    public async Task Update_ShouldValidateSuccessfully_WhenChangingToNewUniqueName()
    {
        // Arrange
        string url = "/api/v1/products";

        ProductViewModel createdProduct = await GetLastCreatedProduct();
        createdProduct!.Name = $"Updated Name {Guid.NewGuid()}";

        ProductViewModel productToUpdate = ProductViewModelBuilder.New()
            .WithId(createdProduct!.Id)
            .WithUniqueName()
            .Build();

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.PutAndReturnResponseAsync(_client, url, productToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion
}