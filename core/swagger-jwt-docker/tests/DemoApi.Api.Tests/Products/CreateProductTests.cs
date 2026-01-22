using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Api.Tests.Common.Helpers;
using DemoApi.Api.Tests.Common.Configuration;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using DemoApi.Tests.Builders.Products;

namespace DemoApi.Api.Tests.Products
{
    [TestCaseOrderer("DemoApi.Api.Tests.Configuration.PriorityOrderer", "DemoApi.Api.Tests")]
    public class CreateProductTests(CustomWebApplicationFactory factory) : ProductApiTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(100)]
        public async Task Create_ShouldReturnCreated_WhenProductIsValid()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New().Build();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeTrue();
            viewModel!.Data.Should().NotBeNull();
        }

        [Fact, TestPriority(101)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductNameIsEmpty()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New().WithEmptyName().Build();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(102)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductNameIsNull()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New().WithNullName().Build();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel!.Errors.Should().Contain("Name is required");
        }

        [Fact, TestPriority(103)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductWeightIsZero()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New().WithZeroWeight().Build();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(104)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenProductWeightIsNegative()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New().WithNegativeWeight().Build();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel!.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel!.Errors.Should().Contain("Weight must be greater than 0");
        }

        [Fact, TestPriority(105)]
        public async Task Create_ShouldReturnCreated_WhenProductNameHasLongLength()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New().WithLongName().Build();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            viewModel.Should().NotBeNull();
            viewModel!.Success.Should().BeTrue();
        }

        [Fact, TestPriority(106)]
        public async Task Create_ShouldReturnCreated_WhenWeightIsLarge()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel productFake = ProductViewModelBuilder.New().WithLargeWeight().Build();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            viewModel.Should().NotBeNull();
            viewModel!.Success.Should().BeTrue();
        }

        [Fact, TestPriority(107)]
        public async Task Create_ShouldReturnBadRequest_WhenDuplicateProductName()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            string duplicateName = "Duplicate Test Product Name";
            ProductViewModel product = ProductViewModelBuilder.New().WithName(duplicateName).Build();

            await HttpClientHelper.PostAndReturnResponseAsync(client, url, product);

            ProductViewModel duplicateProduct = ProductViewModelBuilder.New().WithName(duplicateName).Build();

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, duplicateProduct);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            viewModel.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
            viewModel.Errors.Should().Contain(e => e.Contains("already registered"));
        }

        [Fact, TestPriority(108)]
        public async Task Create_ShouldAccept_ProductsWithDifferentNamesButSimilarWeight()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel product1 = ProductViewModelBuilder.New().WithWeight(1.5).Build();
            ProductViewModel product2 = ProductViewModelBuilder.New().WithWeight(1.5).Build();

            // Act
            (HttpResponseMessage response1, ResponseViewModel? _) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, product1);
            (HttpResponseMessage response2, ResponseViewModel? _) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, product2);

            // Assert
            response1.StatusCode.Should().Be(HttpStatusCode.Created);
            response2.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact, TestPriority(109)]
        public async Task Create_ShouldHandleMultipleConcurrentRequests()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            List<Task<HttpResponseMessage>> tasks = [];

            for (int i = 0; i < 5; i++)
            {
                ProductViewModel product = ProductViewModelBuilder.New()
                    .WithName($"Concurrent Product {Guid.NewGuid()}")
                    .Build();
                tasks.Add(client.PostAsJsonAsync(url, product));
            }

            // Act
            HttpResponseMessage[] results = await Task.WhenAll(tasks);

            // Assert
            int successCount = results.Count(r => r.StatusCode == HttpStatusCode.Created);
            successCount.Should().BeGreaterThanOrEqualTo(1, "at least one concurrent request should succeed");
        }

        [Fact, TestPriority(110)]
        public async Task Create_ShouldReturnPreconditionFailed_WhenPayloadIsNull()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            ProductViewModel? productFake = null;

            // Act
            (HttpResponseMessage response, ResponseViewModel? viewModel) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, productFake);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
            viewModel.Should().NotBeNull();
            viewModel!.Success.Should().BeFalse();
        }

        [Fact, TestPriority(111)]
        public async Task Create_ShouldReturnUnsupportedMediaType_WhenContentTypeIsInvalid()
        {
            // Arrange
            HttpClient client = await GetAuthenticatedClient();
            string url = "/api/v1/products";
            StringContent content = new("invalid-json", Encoding.UTF8, "text/plain");

            // Act
            HttpResponseMessage response = await client.PostAsync(url, content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
        }

        #endregion
    }
}