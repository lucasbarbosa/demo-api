using System.Net;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DemoApi.Api.Controllers;

[ApiController]
public class MainApiController : Controller
{
    #region Properties

    private readonly INotificatorHandler _notificator;
    private readonly IConfiguration? _configuration;

    #endregion

    #region Constructors

    public MainApiController(INotificatorHandler notificator)
    {
        _notificator = notificator;
    }

    public MainApiController(INotificatorHandler notificator, IConfiguration configuration)
    {
        _notificator = notificator;
        _configuration = configuration;
    }

    #endregion

    #region Protected Methods

    protected ActionResult CustomResponse(object? result = null)
    {
        if (result is null or false)
        {
            return NotFound(new ResponseViewModel
            {
                Success = false,
                Errors = GetErrors()
            });
        }

        if (ValidOperation() is false)
        {
            return BadRequest(new ResponseViewModel
            {
                Success = false,
                Errors = GetErrors()
            });
        }

        return Ok(new ResponseViewModel
        {
            Success = true,
            Data = result
        });
    }

    protected ActionResult CustomResponse(HttpStatusCode statusCode, object? result)
    {
        if (ValidOperation() is false)
        {
            if (statusCode == HttpStatusCode.NoContent)
                statusCode = HttpStatusCode.BadRequest;

            return StatusCode((int)statusCode, new ResponseViewModel
            {
                Success = false,
                Errors = GetErrors()
            });
        }

        return (statusCode == HttpStatusCode.NoContent)
            ? NoContent()
            : StatusCode((int)statusCode, new ResponseViewModel
            {
                Success = true,
                Data = result
            });
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        if (ModelState.IsValid is false)
        {
            ErrorModelState(modelState);
            return CustomResponse(HttpStatusCode.PreconditionFailed, new ResponseViewModel
            {
                Success = false,
                Errors = GetErrors()
            });
        }
        else
            return CustomResponse();
    }

    protected ActionResult CustomResponseCreate(object? result)
    {
        if (ValidOperation() is false)
        {
            return BadRequest(new ResponseViewModel
            {
                Success = false,
                Errors = GetErrors()
            });
        }

        return StatusCode((int)HttpStatusCode.Created, new ResponseViewModel
        {
            Success = true,
            Data = result
        });
    }

    protected void ErrorModelState(ModelStateDictionary modelState)
    {
        IEnumerable<ModelError> erros = modelState.Values.SelectMany(e => e.Errors);

        foreach (ModelError erro in erros)
        {
            string errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
            AddError(errorMsg);
        }
    }

    protected void AddError(string error)
    {
        _notificator.AddError(error);
    }

    protected bool ValidOperation()
    {
        return !_notificator.HasErrors();
    }

    protected string[] GetErrors()
    {
        return _notificator.GetErrors().Select(x => x.Message).ToArray();
    }

    #endregion
}