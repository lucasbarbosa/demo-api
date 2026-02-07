using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Api.Tests.Common.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using DemoApi.Tests.Builders.Products;

namespace DemoApi.Api.Tests.Products;

public class ProductApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private const string ValidSecurityKey = "b5b622cd-9f73-43b8-8dce-aab520cf1a2b";

    #region Properties

    protected readonly HttpClient _client = factory.CreateClient();

    #endregion

    #region Protected Methods

    public async ValueTask InitializeAsync()
    {
        using HttpClient tokenClient = factory.CreateClient();
        tokenClient.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

        HttpResponseMessage result = await tokenClient.PostAsync("/api/v1/auth/token", null);
        
        if (!result.IsSuccessStatusCode) return;

        ResponseViewModel? response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

        if (response?.Data != null)
        {
            JsonElement tokenJson = JsonSerializer.Deserialize<JsonElement>(response.Data.ToString()!);
            if (tokenJson.TryGetProperty("accessToken", out JsonElement accessTokenProp))
            {
                string? token = accessTokenProp.GetString();
                if (!string.IsNullOrEmpty(token))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
        }
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;


    protected async Task<ProductViewModel> GetLastCreatedProduct()
    {
        string url = "/api/v1/products";
        ProductViewModel newProduct = ProductViewModelBuilder.New().Build();
        (HttpResponseMessage _, ResponseViewModel? createResponse) = await HttpClientHelper.PostAndReturnResponseAsync(_client, url, newProduct);

        if (createResponse == null || !createResponse.Success)
        {
            string errors = createResponse?.Errors != null ? string.Join(", ", createResponse.Errors) : "Unknown error";
            throw new Exception($"Failed to create product: {errors}");
        }

        ProductViewModel? createdProduct = JsonSerializer.Deserialize<ProductViewModel>(
            createResponse!.Data!.ToString()!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return createdProduct!;
    }

    #endregion
}