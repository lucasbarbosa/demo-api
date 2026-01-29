using DemoApi.Api.Extensions;
using DemoApi.Application.Models;
using DemoApi.Domain.Handlers;
using DemoApi.Domain.Interfaces;
using DemoApi.Infra.CrossCutting.Interfaces;

using FluentAssertions;

using Microsoft.AspNetCore.Http;

using Moq;

using Newtonsoft.Json;

namespace DemoApi.Api.Tests.Common.Middleware;

public class ExceptionMiddlewareTests
{
    #region Public Methods

    [Fact]
    public async Task InvokeAsync_ShouldCallNextDelegate_WhenNoExceptionOccurs()
    {
        // Arrange
        bool nextDelegateCalled = false;

        Task NextDelegate(HttpContext hc)
        {
            nextDelegateCalled = true;
            return Task.CompletedTask;
        }

        ExceptionMiddleware middleware = new(NextDelegate);
        DefaultHttpContext context = new();
        Mock<ILogger> logger = new();
        Mock<INotificatorHandler> notificator = new();

        // Act
        await middleware.InvokeAsync(context, logger.Object, notificator.Object);

        // Assert
        nextDelegateCalled.Should().BeTrue();
        logger.Verify(l => l.LogException(It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturn500_WhenUnhandledExceptionOccurs()
    {
        // Arrange
        InvalidOperationException expectedException = new("Test exception");

        Task NextDelegate(HttpContext hc) => throw expectedException;

        ExceptionMiddleware middleware = new(NextDelegate);
        DefaultHttpContext context = new();
        context.Response.Body = new MemoryStream();

        Mock<ILogger> logger = new();
        Mock<INotificatorHandler> notificator = new();
        notificator.Setup(n => n.GetErrors()).Returns([]);

        // Act
        await middleware.InvokeAsync(context, logger.Object, notificator.Object);

        // Assert
        context.Response.StatusCode.Should().Be(500);
        context.Response.ContentType.Should().Be("application/json");
    }

    [Fact]
    public async Task InvokeAsync_ShouldLogException_WhenExceptionThrown()
    {
        // Arrange
        InvalidOperationException expectedException = new("Test exception");

        Task NextDelegate(HttpContext hc) => throw expectedException;

        ExceptionMiddleware middleware = new(NextDelegate);
        DefaultHttpContext context = new();
        context.Response.Body = new MemoryStream();

        Mock<ILogger> logger = new();
        Mock<INotificatorHandler> notificator = new();
        notificator.Setup(n => n.GetErrors()).Returns([]);

        // Act
        await middleware.InvokeAsync(context, logger.Object, notificator.Object);

        // Assert
        logger.Verify(l => l.LogException(expectedException), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnStandardizedResponse_OnError()
    {
        // Arrange
        InvalidOperationException expectedException = new("Test exception message");

        Task NextDelegate(HttpContext hc) => throw expectedException;

        ExceptionMiddleware middleware = new(NextDelegate);
        DefaultHttpContext context = new();
        context.Response.Body = new MemoryStream();

        Mock<ILogger> logger = new();
        Mock<INotificatorHandler> notificator = new();
        notificator.Setup(n => n.GetErrors()).Returns(
        [
            new Notification("Domain error 1")
        ]);

        // Act
        await middleware.InvokeAsync(context, logger.Object, notificator.Object);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        ResponseViewModel? response = JsonConvert.DeserializeObject<ResponseViewModel>(responseBody);

        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Errors.Should().Contain("Domain error 1");
        response.Errors.Should().Contain("Test exception message");
    }

    [Fact]
    public async Task InvokeAsync_ShouldIncludeDomainErrors_InResponse()
    {
        // Arrange
        Exception expectedException = new("Exception message");

        Task NextDelegate(HttpContext hc) => throw expectedException;

        ExceptionMiddleware middleware = new(NextDelegate);
        DefaultHttpContext context = new();
        context.Response.Body = new MemoryStream();

        Mock<ILogger> logger = new();
        Mock<INotificatorHandler> notificator = new();
        notificator.Setup(n => n.GetErrors()).Returns(
        [
            new Notification("Validation error 1"),
            new Notification("Validation error 2")
        ]);

        // Act
        await middleware.InvokeAsync(context, logger.Object, notificator.Object);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        string responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        ResponseViewModel? response = JsonConvert.DeserializeObject<ResponseViewModel>(responseBody);

        response!.Errors.Should().HaveCount(3);
        response.Errors.Should().Contain("Validation error 1");
        response.Errors.Should().Contain("Validation error 2");
        response.Errors.Should().Contain("Exception message");
    }

    #endregion
}