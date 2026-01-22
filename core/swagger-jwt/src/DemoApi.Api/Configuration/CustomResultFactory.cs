using DemoApi.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using System.Net;

namespace DemoApi.Api.Configuration
{
    public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails? validationProblemDetails)
        {
            var errors = validationProblemDetails?.Errors.SelectMany(x => x.Value).ToList() ?? new List<string>();

            // Heuristic: If any error comes from an Exception (Binding/Format error), return 400 Bad Request.
            // Otherwise (pure validation errors), return 412 Precondition Failed.
            bool isBindingError = context.ModelState.Values.Any(v => v.Errors.Any(e => e.Exception != null)) || 
                                  context.ModelState.Keys.Any(k => k == "id" || k == "Id"); // Fallback for path params if exception is missing

            var statusCode = isBindingError ? HttpStatusCode.BadRequest : HttpStatusCode.PreconditionFailed;

            var response = new ResponseViewModel
            {
                Success = false,
                Errors = errors
            };

            return new ObjectResult(response)
            {
                StatusCode = (int)statusCode
            };
        }
    }
}