using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

using DemoApi.Application.Models;

namespace DemoApi.Api.Tests.Common.Helpers;

public static class HttpClientHelper
{
    #region Public Methods

    public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> GetAndReturnResponseAsync(HttpClient client, string url, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await client.GetAsync(url, cancellationToken);
        
        if (response.Content.Headers.ContentLength == 0)
        {
            return (response, null);
        }

        try
        {
            ResponseViewModel? viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: cancellationToken);
            return (response, viewModel);
        }
        catch (JsonException)
        {
            return (response, null);
        }
    }

    public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> PostAndReturnResponseAsync(HttpClient client, string url, object? request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await client.PostAsJsonAsync(url, request, cancellationToken);
        
        if (response.Content.Headers.ContentLength == 0)
        {
            return (response, null);
        }

        try
        {
            ResponseViewModel? viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: cancellationToken);
            return (response, viewModel);
        }
        catch (JsonException)
        {
            return (response, null);
        }
    }

    public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> PutAndReturnResponseAsync(HttpClient client, string url, object? request, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await client.PutAsJsonAsync(url, request, cancellationToken);
        ResponseViewModel? viewModel = null;
        
        if (!response.IsSuccessStatusCode && response.Content.Headers.ContentLength > 0)
        {
            try
            {
                viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: cancellationToken);
            }
            catch (JsonException) { }
        }
        else if (response.IsSuccessStatusCode && response.StatusCode != System.Net.HttpStatusCode.NoContent && response.Content.Headers.ContentLength > 0)
        {
             // Try read if success and not NoContent (204)
            try
            {
                viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: cancellationToken);
            }
            catch (JsonException) { }
        }

        return (response, viewModel);
    }

    public static async Task<(HttpResponseMessage response, ResponseViewModel? viewModel)> DeleteAndReturnResponseAsync(HttpClient client, string url, CancellationToken cancellationToken = default)
    {
        HttpResponseMessage response = await client.DeleteAsync(url, cancellationToken);
        ResponseViewModel? viewModel = null;
        
        if (!response.IsSuccessStatusCode && response.Content.Headers.ContentLength > 0)
        {
            try
            {
                viewModel = await response.Content.ReadFromJsonAsync<ResponseViewModel>(cancellationToken: cancellationToken);
            }
            catch (JsonException) { }
        }

        return (response, viewModel);
    }

    #endregion
}