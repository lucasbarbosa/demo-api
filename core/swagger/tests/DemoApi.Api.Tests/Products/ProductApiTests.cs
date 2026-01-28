namespace DemoApi.Api.Tests.Products;

using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Api.Tests.Common.Helpers;
using DemoApi.Application.Models;
using DemoApi.Application.Models.Products;
using DemoApi.Tests.Builders.Products;
using Xunit;
using System.Text.Json;
public class ProductApiTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    #region Properties

    protected readonly HttpClient _client = factory.CreateClient();

    #endregion
    
    #region Protected Methods


    protected async Task<ProductViewModel> GetLastCreatedProduct()
    {
        string url = "/api/v1/products";
        ProductViewModel newProduct = ProductViewModelBuilder.New().Build();
        (HttpResponseMessage _, ResponseViewModel? createResponse) = await HttpClientHelper.PostAndReturnResponseAsync(_client, url, newProduct);

        if (!createResponse!.Success)
        {
            throw new Exception($"Failed to create product: {string.Join(", ", createResponse.Errors)}");
        }

        ProductViewModel? createdProduct = JsonSerializer.Deserialize<ProductViewModel>(
            createResponse!.Data!.ToString()!,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return createdProduct!;
    }

    #endregion
}