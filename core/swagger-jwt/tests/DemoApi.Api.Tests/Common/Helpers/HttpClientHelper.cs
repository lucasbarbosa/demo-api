using DemoApi.Application.Models;
using System.Net.Http.Json;

namespace DemoApi.Api.Tests.Common.Helpers
{
    public static class HttpClientHelper
    {
        #region Public Methods

        public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> GetAndReturnResponseAsync(HttpClient client, string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            ResponseViewModel? viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>();

            return (response, viewModel);
        }

        public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> PostAndReturnResponseAsync(HttpClient client, string url, object? request)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(url, request);
            ResponseViewModel? viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>();

            return (response, viewModel);
        }

        public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> PutAndReturnResponseAsync(HttpClient client, string url, object? request)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(url, request);
            ResponseViewModel? viewModel = (!response.IsSuccessStatusCode) ? await response.Content.ReadFromJsonAsync<ResponseViewModel>() : null;

            return (response, viewModel);
        }

        public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> DeleteAndReturnResponseAsync(HttpClient client, string url)
        {
            HttpResponseMessage response = await client.DeleteAsync(url);
            ResponseViewModel? viewModel = (!response.IsSuccessStatusCode) ? await response.Content.ReadFromJsonAsync<ResponseViewModel>() : null;

            return (response, viewModel);
        }

        #endregion
    }
}