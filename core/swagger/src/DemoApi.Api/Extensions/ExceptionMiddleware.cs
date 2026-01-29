using System.Net;
using System.Text;

using ILogger = DemoApi.Infra.CrossCutting.Interfaces.ILogger;

using Newtonsoft.Json;

namespace DemoApi.Api.Extensions;

public class ExceptionMiddleware(RequestDelegate next)
{
    #region Properties

    private readonly RequestDelegate _next = next;

    #endregion

    #region Public Methods

    public async Task InvokeAsync(HttpContext httpContext, ILogger logger, INotificatorHandler notificator)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, logger, notificator);
        }
    }

    #endregion

    #region Private Methods

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger logger, INotificatorHandler notificator)
    {
        logger.LogException(exception);

        var responseBody = new ResponseViewModel
        {
            Success = false,
            Errors = notificator.GetErrors().Select(x => x.Message).ToList()
        };

        responseBody.Errors.Add(exception.Message);

        var responseJson = JsonConvert.SerializeObject(responseBody);
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(responseJson, Encoding.UTF8);
    }

    #endregion
}