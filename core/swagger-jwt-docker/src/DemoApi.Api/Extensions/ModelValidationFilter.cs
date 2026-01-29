using System.Net;

using Microsoft.AspNetCore.Mvc.Filters;

namespace DemoApi.Api.Extensions;

public class ModelValidationFilter(INotificatorHandler notificator) : IActionFilter
{
    private readonly INotificatorHandler _notificator = notificator;

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => e.ErrorMessage))
                .ToList();

            // Check if errors contain Data Annotation validation messages
            var hasDataAnnotationErrors = errors.Any(e =>
                e.Contains("required", StringComparison.OrdinalIgnoreCase) ||
                e.Contains("must be", StringComparison.OrdinalIgnoreCase) ||
                e.Contains("range", StringComparison.OrdinalIgnoreCase));

            foreach (var error in errors)
            {
                _notificator.AddError(error);
            }

            // Use 412 for Data Annotation errors, 400 for binding errors
            var statusCode = hasDataAnnotationErrors
                ? (int)HttpStatusCode.PreconditionFailed
                : (int)HttpStatusCode.BadRequest;

            context.Result = new ObjectResult(new ResponseViewModel
            {
                Success = false,
                Errors = _notificator.GetErrors().Select(x => x.Message).ToArray()
            })
            {
                StatusCode = statusCode
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}