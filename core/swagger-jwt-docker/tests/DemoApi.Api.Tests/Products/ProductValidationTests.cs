using System.Net;

using DemoApi.Api.Tests.Common.Configuration;
using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Api.Tests.Common.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using DemoApi.Tests.Builders.Products;

using FluentAssertions;

namespace DemoApi.Api.Tests.Products;

[TestCaseOrderer("DemoApi.Api.Tests.Configuration.PriorityOrderer", "DemoApi.Api.Tests")]
public class ProductValidationTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
{
    #region Name Validation Tests

    [Fact, TestPriority(150)]
    public async Task Create_ShouldReturnPreconditionFailed_WhenProductNameIsWhitespace()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithWhitespaceName()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().Contain("Name is required");
    }

    [Fact, TestPriority(151)]
    public async Task Create_ShouldReturnCreated_WhenProductNameContainsSpecialCharacters()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithSpecialCharactersName()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response!.Data.Should().NotBeNull();
    }

    [Fact, TestPriority(152)]
    public async Task Create_ShouldReturnCreated_WhenProductNameIsVeryLong()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithLongName(500)
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response!.Data.Should().NotBeNull();
    }

    [Fact, TestPriority(153)]
    public async Task Create_ShouldReturnCreated_WhenProductNameContainsUnicode()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithUnicodeName()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response!.Data.Should().NotBeNull();
    }

    #endregion

    #region Weight Validation Tests

    [Theory, TestPriority(154)]
    [InlineData(0.01)]
    [InlineData(0.1)]
    [InlineData(1.0)]
    [InlineData(10.5)]
    [InlineData(100.99)]
    [InlineData(999.99)]
    public async Task Create_ShouldReturnCreated_WhenProductWeightIsValid(double weight)
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithWeight(weight)
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response!.Data.Should().NotBeNull();
    }

    [Theory, TestPriority(155)]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    [InlineData(-100)]
    public async Task Create_ShouldReturnPreconditionFailed_WhenProductWeightIsNegative(double weight)
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithWeight(weight)
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().Contain("Weight must be greater than 0");
    }

    [Fact, TestPriority(156)]
    public async Task Create_ShouldReturnCreated_WhenProductWeightHasManyDecimalPlaces()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithPreciseWeight()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response!.Data.Should().NotBeNull();
    }

    #endregion

    #region Multiple Validation Errors Tests

    [Fact, TestPriority(157)]
    public async Task Create_ShouldReturnPreconditionFailed_WithMultipleErrors_WhenNameAndWeightAreInvalid()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithEmptyName()
            .WithNegativeWeight()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        response!.Errors.Should().Contain("Name is required");
        response!.Errors.Should().Contain("Weight must be greater than 0");
    }

    [Fact, TestPriority(158)]
    public async Task Create_ShouldReturnPreconditionFailed_WithMultipleErrors_WhenNameIsWhitespaceAndWeightIsZero()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithWhitespaceName()
            .WithZeroWeight()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        response!.Errors.Should().Contain("Name is required");
        response!.Errors.Should().Contain("Weight must be greater than 0");
    }

    #endregion

    #region Update Validation Tests

    [Fact, TestPriority(159)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductNameIsWhitespace()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithWhitespaceName()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().Contain("Name is required");
    }

    [Theory, TestPriority(160)]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(0)]
    public async Task Update_ShouldReturnPreconditionFailed_WhenProductWeightIsInvalid(double weight)
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithWeight(weight)
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().Contain("Weight must be greater than 0");
    }

    [Fact, TestPriority(161)]
    public async Task Update_ShouldReturnPreconditionFailed_WithMultipleErrors_WhenAllFieldsAreInvalid()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithEmptyName()
            .WithNegativeWeight()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PutAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response!.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response!.Errors.Should().HaveCountGreaterThanOrEqualTo(2);
        response!.Errors.Should().Contain("Name is required");
        response!.Errors.Should().Contain("Weight must be greater than 0");
    }

    #endregion

    #region Boundary Tests

    [Fact, TestPriority(162)]
    public async Task Create_ShouldReturnCreated_WhenProductWeightIsVerySmallPositive()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithVerySmallPositiveWeight()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response!.Data.Should().NotBeNull();
    }

    [Fact, TestPriority(163)]
    public async Task Create_ShouldReturnCreated_WhenProductNameIsSingleCharacter()
    {
        // Arrange
        HttpClient client = await GetAuthenticatedClient();
        string url = "/api/v1/products";
        ProductViewModel productFake = ProductViewModelBuilder.New()
            .WithSingleCharacterName()
            .Build();

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response!.Data.Should().NotBeNull();
    }

    #endregion
}