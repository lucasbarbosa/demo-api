using FluentValidation;

using Microsoft.AspNetCore.Mvc.Filters;

namespace DemoApi.Api.Extensions;

public class FluentValidationFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (object? argument in context.ActionArguments.Values)
        {
            if (argument == null) continue;

            Type argumentType = argument!.GetType();
            Type validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (_serviceProvider.GetService(validatorType) is IValidator validator)
            {
                ValidationContext<object> validationContext = new(argument!);
                FluentValidation.Results.ValidationResult validationResult = await validator.ValidateAsync(validationContext);

                if (!validationResult.IsValid)
                {
                    foreach (FluentValidation.Results.ValidationFailure error in validationResult.Errors)
                    {
                        context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                    }
                }
            }
        }

        await next();
    }
}