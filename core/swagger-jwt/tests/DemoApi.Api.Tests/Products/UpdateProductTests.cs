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

[TestCaseOrderer("DemoApi.Api.Tests.Configuration.PriorityOrderer", "DemoApi.Api.Tests")]
public class UpdateProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
{
    #region Public Methods

    [Fact, TestPriority(300)]
    public async Task Update_ShouldReturnNoContent_WhenProductIsValid()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = await GetLastCreatedProduct();
        ProductViewModel productToUpdate = ProductViewModelBuilder.New()
            .WithId(productFake.Id)
            .WithName("Updated Product Name")
            .WithWeight(5.0)
            .Build();

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productToUpdate);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(301)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithEmptyName()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().Contain("Name is required");
    }

    [Fact, TestPriority(302)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsNull()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithNullName()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().Contain("Name is required");
    }

    [Fact, TestPriority(303)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsZero()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithZeroWeight()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().Contain("Weight must be greater than 0");
    }

    [Fact, TestPriority(304)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsNegative()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithNegativeWeight()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().Contain("Weight must be greater than 0");
    }

    [Fact, TestPriority(305)]
    public async Task Update_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithNonExistentId()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response?.Should().NotBeNull();
        response?.Success.Should().BeFalse();
    }

    [Fact, TestPriority(306)]
    public async Task Update_ShouldReturnNotFound_WhenProductIdIsZero()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithIdZero()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(307)]
    public async Task Update_ShouldAllowChangingWeightOnly()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        ProductViewModel product = await GetLastCreatedProduct();
        string url = "/api/v1/products";
        ProductViewModel updatedProduct = ProductViewModelBuilder.New()
            .WithId(product.Id)
            .WithName(product.Name!)
            .WithWeight(product.Weight + 1.0)
            .Build();

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, updatedProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(308)]
    public async Task Update_ShouldAllowChangingNameOnly()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        ProductViewModel product = await GetLastCreatedProduct();
        string url = "/api/v1/products";
        ProductViewModel updatedProduct = ProductViewModelBuilder.New()
            .WithId(product.Id)
            .WithName("New Name Only")
            .WithWeight(product.Weight)
            .Build();

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, updatedProduct);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(309)]
    public async Task Update_ShouldHandleConcurrentUpdates()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        ProductViewModel product = await GetLastCreatedProduct();
        string updateUrl = "/api/v1/products";

        List<Task<HttpResponseMessage>> tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 3; i++)
        {
            ProductViewModel productToUpdate = ProductViewModelBuilder.New()
                .WithId(product.Id)
                .WithName($"Concurrent Update {i}")
                .WithWeight(5.0)
                .Build();
            tasks.Add(client.PutAsJsonAsync(updateUrl, productToUpdate));
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
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";

        ProductViewModel product = await GetLastCreatedProduct();
        product.Name = $"Updated Name {Guid.NewGuid()}";

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, product);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    #endregion
}