using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Application.Models;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DemoApi.Api.Tests.Auth
{
    public class AuthTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        #region Properties

        protected readonly CustomWebApplicationFactory _factory = factory;
        protected readonly HttpClient _client = factory.CreateClient();
        protected const string ValidSecurityKey = "b5b622cd-9f73-43b8-8dce-aab520cf1a2b";

        #endregion

        #region Protected Methods

        protected async Task<HttpClient> GetAuthenticatedClient()
        {
            var tokenClient = _factory.CreateClient();
            tokenClient.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);
            var result = await tokenClient.PostAsync("/api/v1/auth/token", null);
            var response = await result.Content.ReadFromJsonAsync<ResponseViewModel>();

            string token = string.Empty;
            if (response?.Data != null)
            {
                var tokenJson = JObject.Parse(response.Data.ToString()!);
                var accessToken = tokenJson["accessToken"];
                token = accessToken?.ToString() ?? string.Empty;
            }

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        #endregion
    }
}