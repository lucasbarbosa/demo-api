using System.Net.Http.Json;
using Xunit;

using DemoApi.Application.Models;

namespace DemoApi.Api.Tests.Common.Helpers;

public static class HttpClientHelper
{
    #region Public Methods

    public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> GetAndReturnResponseAsync(HttpClient client, string url)
    {
        HttpResponseMessage response = await client.GetAsync(url, TestContext.Current.CancellationToken);
        ResponseViewModel? viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: TestContext.Current.CancellationToken);

        return (response, viewModel);
    }

    public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> PostAndReturnResponseAsync(HttpClient client, string url, object? request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(url, request, cancellationToken);
        ResponseViewModel? viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: cancellationToken);

        return (response, viewModel);
    }

    public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> PutAndReturnResponseAsync(HttpClient client, string url, object? request)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(url, request, cancellationToken: TestContext.Current.CancellationToken);
        ResponseViewModel? viewModel = (!response.IsSuccessStatusCode) ? await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: TestContext.Current.CancellationToken) : null;

        return (response, viewModel);
    }

    public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> DeleteAndReturnResponseAsync(HttpClient client, string url)
    {
        HttpResponseMessage response = await client.DeleteAsync(url, TestContext.Current.CancellationToken);
        ResponseViewModel? viewModel = (!response.IsSuccessStatusCode) ? await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: TestContext.Current.CancellationToken) : null;

        return (response, viewModel);
    }

    #endregion
}