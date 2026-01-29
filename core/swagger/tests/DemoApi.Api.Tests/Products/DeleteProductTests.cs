using System.Net;

using DemoApi.Api.Tests.Common.Configuration;
using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Api.Tests.Common.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;

using FluentAssertions;

namespace DemoApi.Api.Tests.Products;

[TestCaseOrderer("DemoApi.Api.Tests.Configuration.PriorityOrderer", "DemoApi.Api.Tests")]
public class DeleteProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
{
    #region Public Methods

    [Fact, TestPriority(400)]
    public async Task Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        string url = "/api/v1/products/999999";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeFalse();
    }

    [Fact, TestPriority(401)]
    public async Task Delete_ShouldReturnBadRequest_WhenIdIsNotNumeric()
    {
        // Arrange
        string url = "/api/v1/products/XYZ";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        viewModel?.Should().NotBeNull();
        viewModel?.Success.Should().BeFalse();
        viewModel?.Errors.Should().NotBeEmpty();
    }

    [Fact, TestPriority(402)]
    public async Task Delete_ShouldReturnNoContent_WhenProductExists()
    {
        // Arrange
        ProductViewModel product = await GetLastCreatedProduct();
        string url = $"/api/v1/products/{product.Id}";

        // Act
        (HttpResponseMessage response, _) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact, TestPriority(403)]
    public async Task Delete_ShouldReturnNotFound_WhenIdIsZero()
    {
        // Arrange
        string url = "/api/v1/products/0";

        // Act
        (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        viewModel.Should().NotBeNull();
        viewModel!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(404)]
    public async Task Delete_ShouldReturnNotFound_WhenDeletingSameProductTwice()
    {
        // Arrange
        ProductViewModel product = await GetLastCreatedProduct();
        string url = $"/api/v1/products/{product.Id}";

        // Act
        (HttpResponseMessage firstResponse, _) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

        // Assert
        firstResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act
        (HttpResponseMessage secondResponse, ResponseViewModel? secondViewModel) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, url);

        // Assert
        secondResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        secondViewModel.Should().NotBeNull();
        secondViewModel!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(405)]
    public async Task Delete_ShouldMakeProductUnavailable_WhenDeleted()
    {
        // Arrange
        ProductViewModel product = await GetLastCreatedProduct();
        string deleteUrl = $"/api/v1/products/{product.Id}";
        string getUrl = $"/api/v1/products/{product.Id}";

        // Act
        (HttpResponseMessage deleteResponse, ResponseViewModel? _) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, deleteUrl);
        (HttpResponseMessage getResponse, ResponseViewModel? getViewModel) = await HttpClientHelper.GetAndReturnResponseAsync(_client, getUrl);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        getViewModel.Should().NotBeNull();
        getViewModel!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(406)]
    public async Task Delete_ShouldPreventUpdate_AfterDeletion()
    {
        // Arrange
        ProductViewModel product = await GetLastCreatedProduct();
        string deleteUrl = $"/api/v1/products/{product.Id}";
        string updateUrl = "/api/v1/products";

        // Act
        (HttpResponseMessage deleteResponse, ResponseViewModel? _) = await HttpClientHelper.DeleteAndReturnResponseAsync(_client, deleteUrl);

        product.Name = "Trying to update deleted product";
        (HttpResponseMessage updateResponse, ResponseViewModel? updateViewModel) = await HttpClientHelper.PutAndReturnResponseAsync(_client, updateUrl, product);

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        updateViewModel.Should().NotBeNull();
        updateViewModel!.Success.Should().BeFalse();
    }

    #endregion
}