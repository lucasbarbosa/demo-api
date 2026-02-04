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

public class GetProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
{
    #region Public Methods

    [Fact, TestPriority(200)]
    public async Task GetAll_ShouldReturnOk_WhenProductsExist()
    {
        // Arrange
        string url = "/api/v1/products";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeTrue();
        viewModel?.Data.Should().NotBeNull();
    }

    [Fact, TestPriority(201)]
    public async Task GetById_ShouldReturnOk_WhenProductExists()
    {
        // Arrange
        ProductViewModel product = await GetLastCreatedProduct();
        string url = $"/api/v1/products/{product.Id}";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeTrue();
        viewModel?.Data.Should().NotBeNull();
    }

    [Fact, TestPriority(202)]
    public async Task GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        string url = "/api/v1/products/999999";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeFalse();
    }

    [Fact, TestPriority(203)]
    public async Task GetById_ShouldReturnBadRequest_WhenIdIsNotNumeric()
    {
        // Arrange
        string url = "/api/v1/products/ABC";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeFalse();
        viewModel?.Errors.Should().NotBeEmpty();
        viewModel?.Errors.Should().Contain(e => e.Contains("The value 'ABC' is not valid"));
    }

    [Fact, TestPriority(204)]
    public async Task GetById_ShouldReturnBadRequest_WhenIdIsNegative()
    {
        // Arrange
        string url = "/api/v1/products/-1";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeFalse();
        viewModel?.Errors.Should().NotBeEmpty();
    }

    [Fact, TestPriority(205)]
    public async Task GetById_ShouldReturnBadRequest_WhenIdIsDecimal()
    {
        // Arrange
        string url = "/api/v1/products/1.5";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeFalse();
        viewModel?.Errors.Should().NotBeEmpty();
    }

    [Fact, TestPriority(206)]
    public async Task GetById_ShouldReturnBadRequest_WhenIdContainsSpecialCharacters()
    {
        // Arrange
        string url = "/api/v1/products/@#$";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeFalse();
        viewModel?.Errors.Should().NotBeEmpty();
    }

    [Fact, TestPriority(207)]
    public async Task GetById_ShouldReturnNotFound_WhenIdIsZero()
    {
        // Arrange
        string url = "/api/v1/products/0";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        viewModel.Should().NotBeNull();
        viewModel!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(208)]
    public async Task GetById_ShouldReturnNotFound_WhenIdIsMaxValue()
    {
        // Arrange
        string url = $"/api/v1/products/{uint.MaxValue}";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        viewModel.Should().NotBeNull();
        viewModel!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(209)]
    public async Task GetAll_ShouldReturnOk_WithMultipleProducts()
    {
        // Arrange
        string url = "/api/v1/products";

        await _client.PostAsJsonAsync(url, ProductViewModelBuilder.New().Build(), TestContext.Current.CancellationToken);
        await _client.PostAsJsonAsync(url, ProductViewModelBuilder.New().Build(), TestContext.Current.CancellationToken);
        await _client.PostAsJsonAsync(url, ProductViewModelBuilder.New().Build(), TestContext.Current.CancellationToken);

        // Act
        HttpResponseMessage response = await _client.GetAsync(url, TestContext.Current.CancellationToken);
        ProductListResponse? productList = await response.Content.ReadFromJsonAsync<ProductListResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        productList.Should().NotBeNull();
        productList!.Success.Should().BeTrue();
        productList.Data.Should().NotBeNull();
        productList.Data.Should().HaveCountGreaterThanOrEqualTo(3);
    }

    [Fact, TestPriority(210)]
    public async Task GetAll_ShouldHandleConcurrentReads()
    {
        // Arrange
        string url = "/api/v1/products";
        List<Task<HttpResponseMessage>> tasks = [];

        for (int i = 0; i < 10; i++)
        {
            tasks.Add(_client.GetAsync(url, TestContext.Current.CancellationToken));
        }

        // Act
        HttpResponseMessage[] results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllSatisfy(r => r.StatusCode.Should().Be(HttpStatusCode.OK));
    }

    #endregion
}