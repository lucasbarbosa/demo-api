using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Api.Tests.Common.Configuration;
using DemoApi.Application.Models;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DemoApi.Api.Tests.Auth
{
    [TestCaseOrderer("DemoApi.Api.Tests.Configuration.PriorityOrderer", "DemoApi.Api.Tests")]
    public class AuthenticatedEndpointTests(CustomWebApplicationFactory factory) : AuthTests(factory)
    {
        #region Public Methods

        [Fact, TestPriority(600)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            string url = "/api/v1/products";

            // Act
            HttpResponseMessage result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(601)]
        public async Task GetProducts_ShouldReturnOk_WhenValidTokenProvided()
        {
            // Arrange
            string token = await GetValidToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = "/api/v1/products";

            // Act
            HttpResponseMessage result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact, TestPriority(602)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenTokenIsInvalid()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "INVALID_TOKEN");

            string url = "/api/v1/products";

            // Act
            HttpResponseMessage result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(603)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenTokenIsMalformed()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.invalid.signature");

            string url = "/api/v1/products";

            // Act
            HttpResponseMessage result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(604)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenTokenHasWrongScheme()
        {
            // Arrange
            string token = await GetValidToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);

            string url = "/api/v1/products";

            // Act
            HttpResponseMessage result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(605)]
        public async Task GetProducts_ShouldReturnUnauthorized_WhenAuthorizationHeaderIsMissing()
        {
            // Arrange
            string url = "/api/v1/products";

            // Act
            HttpResponseMessage result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(606)]
        public async Task CreateProduct_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            string url = "/api/v1/products";
            var product = new { name = "Test Product", weight = 1.5 };

            // Act
            HttpResponseMessage result = await _client.PostAsJsonAsync(url, product);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(607)]
        public async Task UpdateProduct_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            string url = "/api/v1/products";
            var product = new { id = 1, name = "Updated Product", weight = 2.5 };

            // Act
            HttpResponseMessage result = await _client.PutAsJsonAsync(url, product);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(608)]
        public async Task DeleteProduct_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            string url = "/api/v1/products/1";

            // Act
            HttpResponseMessage result = await _client.DeleteAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(609)]
        public async Task GetProductById_ShouldReturnUnauthorized_WhenNoTokenProvided()
        {
            // Arrange
            string url = "/api/v1/products/1";

            // Act
            HttpResponseMessage result = await _client.GetAsync(url);

            // Assert
            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact, TestPriority(610)]
        public async Task MultipleRequests_ShouldWork_WithSameToken()
        {
            // Arrange
            string token = await GetValidToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = "/api/v1/products";

            // Act
            HttpResponseMessage result1 = await _client.GetAsync(url);
            HttpResponseMessage result2 = await _client.GetAsync(url);
            HttpResponseMessage result3 = await _client.GetAsync(url);

            // Assert
            result1.StatusCode.Should().Be(HttpStatusCode.OK);
            result2.StatusCode.Should().Be(HttpStatusCode.OK);
            result3.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        #endregion

        #region Private Methods

        private async Task<string> GetValidToken()
        {
            HttpClient tokenClient = _factory.CreateClient();
            tokenClient.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

            HttpResponseMessage result = await tokenClient.PostAsync("/api/v1/auth/token", null);
            ResponseViewModel? response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            JsonElement tokenJson = JsonSerializer.Deserialize<JsonElement>(
                response!.Data!.ToString()!);

            return tokenJson.GetProperty("accessToken").GetString()!;
        }

        #endregion
    }
}