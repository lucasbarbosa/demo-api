using System.Net;
using Xunit;

using DemoApi.Api.Tests.Common.Configuration;
using DemoApi.Api.Tests.Common.Factories;
using DemoApi.Api.Tests.Common.Helpers;
using DemoApi.Application.Models;

using FluentAssertions;

namespace DemoApi.Api.Tests.Auth;

public class GenerateTokenTests(CustomWebApplicationFactory factory) : AuthTests(factory)
{
    #region Public Methods

    [Fact, TestPriority(500)]
    public async Task GenerateToken_ShouldReturnOk_WhenSecurityKeyIsValid()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Should().NotBeNull();
        response!.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
    }

    [Fact, TestPriority(501)]
    public async Task GenerateToken_ShouldReturnTokenViewModel_WithAllRequiredFields()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        response.Should().NotBeNull();
        response!.Data.Should().NotBeNull();

        string? tokenData = response.Data.ToString();
        tokenData.Should().Contain("accessToken");
        tokenData.Should().Contain("tokenType");
        tokenData.Should().Contain("expiresIn");
        tokenData.Should().Contain("created");
        tokenData.Should().Contain("expires");
    }

    [Fact, TestPriority(502)]
    public async Task GenerateToken_ShouldReturnUnauthorized_WhenSecurityKeyIsInvalid()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        client.DefaultRequestHeaders.Add("X-Security-Key", "INVALID_KEY");

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Errors.Should().Contain("Invalid security key. Authentication failed.");
    }

    [Fact, TestPriority(503)]
    public async Task GenerateToken_ShouldReturnPreconditionFailed_WhenSecurityKeyIsEmpty()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        client.DefaultRequestHeaders.Add("X-Security-Key", string.Empty);

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(504)]
    public async Task GenerateToken_ShouldReturnPreconditionFailed_WhenSecurityKeyIsWhitespace()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        client.DefaultRequestHeaders.Add("X-Security-Key", "   ");

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(505)]
    public async Task GenerateToken_ShouldReturnPreconditionFailed_WhenSecurityKeyHeaderIsMissing()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.PreconditionFailed);
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
    }

    [Fact, TestPriority(506)]
    public async Task GenerateToken_ShouldReturnDifferentTokens_WhenCalledMultipleTimes()
    {
        // Arrange
        HttpClient client1 = _factory.CreateClient();
        HttpClient client2 = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        client1.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);
        client2.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

        // Act
        (HttpResponseMessage _, ResponseViewModel? response1) = await HttpClientHelper.PostAndReturnResponseAsync(client1, url, null, TestContext.Current.CancellationToken);

        await Task.Delay(1000, TestContext.Current.CancellationToken);

        (HttpResponseMessage _, ResponseViewModel? response2) = await HttpClientHelper.PostAndReturnResponseAsync(client2, url, null, TestContext.Current.CancellationToken);

        // Assert
        string? token1 = response1!.Data!.ToString();
        string? token2 = response2!.Data!.ToString();

        token1.Should().NotBe(token2);
    }

    [Fact, TestPriority(507)]
    public async Task GenerateToken_ShouldReturnUnauthorized_WhenSecurityKeyHasExtraCharacters()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey + "EXTRA");

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Errors.Should().Contain("Invalid security key. Authentication failed.");
    }

    [Fact, TestPriority(508)]
    public async Task GenerateToken_ShouldReturnUnauthorized_WhenSecurityKeyIsTruncated()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        string truncatedKey = ValidSecurityKey[..^5];
        client.DefaultRequestHeaders.Add("X-Security-Key", truncatedKey);

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Errors.Should().Contain("Invalid security key. Authentication failed.");
    }

    [Fact, TestPriority(509)]
    public async Task GenerateToken_ShouldBeCaseSensitive_ForSecurityKey()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        string lowerCaseKey = ValidSecurityKey.ToLower();
        client.DefaultRequestHeaders.Add("X-Security-Key", lowerCaseKey);

        bool hasUpperCase = ValidSecurityKey.Any(char.IsUpper);
        HttpStatusCode expectedStatusCode = hasUpperCase ? HttpStatusCode.Unauthorized : HttpStatusCode.OK;

        // Act
        (HttpResponseMessage result, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        result.StatusCode.Should().Be(expectedStatusCode);
        response.Should().NotBeNull();

        if (hasUpperCase)
        {
            response!.Success.Should().BeFalse();
            response.Errors.Should().Contain("Invalid security key. Authentication failed.");
        }
        else
        {
            response!.Success.Should().BeTrue();
            response.Data.Should().NotBeNull();
        }
    }

    [Fact, TestPriority(510)]
    public async Task GenerateToken_ShouldReturnToken_WithBearerTokenType()
    {
        // Arrange
        HttpClient client = _factory.CreateClient();
        string url = "/api/v1/auth/token";
        client.DefaultRequestHeaders.Add("X-Security-Key", ValidSecurityKey);

        // Act
        (_, ResponseViewModel? response) = await HttpClientHelper.PostAndReturnResponseAsync(client, url, null, TestContext.Current.CancellationToken);

        // Assert
        response.Should().NotBeNull();
        string? tokenData = response!.Data!.ToString();
        tokenData.Should().Contain("Bearer");
    }

    #endregion
}