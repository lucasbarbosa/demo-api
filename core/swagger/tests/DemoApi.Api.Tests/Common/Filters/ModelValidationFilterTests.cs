namespace DemoApi.Api.Tests.Common.Filters;

using DemoApi.Api.Extensions;
using DemoApi.Application.Models;
using DemoApi.Domain.Handlers;
using DemoApi.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

public class ModelValidationFilterTests
{
    #region Public Methods

    [Fact]
    public void OnActionExecuting_ShouldNotModifyContext_WhenModelStateIsValid()
    {
        // Arrange
        Mock<INotificatorHandler> notificator = new();
        ModelValidationFilter filter = new(notificator.Object);

        ActionContext actionContext = new(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor(),
            new ModelStateDictionary()
        );

        ActionExecutingContext context = new(
            actionContext,
            [],
            new Dictionary<string, object?>(),
            new Mock<Controller>().Object
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        context.Result.Should().BeNull();
        notificator.Verify(n => n.AddError(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void OnActionExecuting_ShouldReturn412_WhenDataAnnotationError()
    {
        // Arrange
        NotificatorHandler notificator = new();
        ModelValidationFilter filter = new(notificator);

        ModelStateDictionary modelState = new();
        modelState.AddModelError("Name", "Name is required");

        ActionContext actionContext = new(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor(),
            modelState
        );

        ActionExecutingContext context = new(
            actionContext,
            [],
            new Dictionary<string, object?>(),
            new Mock<Controller>().Object
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        context.Result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = context.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(412);

        ResponseViewModel? response = objectResult.Value as ResponseViewModel;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Errors.Should().Contain("Name is required");
    }

    [Fact]
    public void OnActionExecuting_ShouldReturn400_WhenModelBindingError()
    {
        // Arrange
        NotificatorHandler notificator = new();
        ModelValidationFilter filter = new(notificator);

        ModelStateDictionary modelState = new();
        modelState.AddModelError("Id", "The value 'ABC' is not valid.");

        ActionContext actionContext = new(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor(),
            modelState
        );

        ActionExecutingContext context = new(
            actionContext,
            [],
            new Dictionary<string, object?>(),
            new Mock<Controller>().Object
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        context.Result.Should().BeOfType<ObjectResult>();
        ObjectResult? objectResult = context.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(400);

        ResponseViewModel? response = objectResult.Value as ResponseViewModel;
        response.Should().NotBeNull();
        response!.Success.Should().BeFalse();
        response.Errors.Should().Contain("The value 'ABC' is not valid.");
    }

    [Fact]
    public void OnActionExecuting_ShouldAddAllErrors_ToNotificator()
    {
        // Arrange
        NotificatorHandler notificator = new();
        ModelValidationFilter filter = new(notificator);

        ModelStateDictionary modelState = new();
        modelState.AddModelError("Name", "Name is required");
        modelState.AddModelError("Weight", "Weight must be greater than 0");

        ActionContext actionContext = new(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor(),
            modelState
        );

        ActionExecutingContext context = new(
            actionContext,
            [],
            new Dictionary<string, object?>(),
            new Mock<Controller>().Object
        );

        // Act
        filter.OnActionExecuting(context);

        // Assert
        notificator.GetErrors().Should().HaveCount(2);
        notificator.GetErrors().Select(static e => e.Message).Should().Contain(
        [
            "Name is required",
            "Weight must be greater than 0"
        ]);
    }

    [Fact]
    public void OnActionExecuted_ShouldNotThrowException()
    {
        // Arrange
        Mock<INotificatorHandler> notificator = new();
        ModelValidationFilter filter = new(notificator.Object);

        ActionContext actionContext = new(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor()
        );

        ActionExecutedContext context = new(
            actionContext,
            [],
            new Mock<Controller>().Object
        );

        // Act & Assert
        void Act() => filter.OnActionExecuted(context);
        ((Action)Act).Should().NotThrow();
    }

    #endregion
}