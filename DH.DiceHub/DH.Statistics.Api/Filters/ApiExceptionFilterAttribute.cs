using Microsoft.AspNetCore.Mvc.Filters;
using DH.Statistics.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using DH.Statistics.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace DH.Statistics.Api.Filters;

public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
{
    readonly ILogger<ApiExceptionFilterAttribute> _logger;
    readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
    {
        _logger = logger;
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(ValidationErrorsException), HandleValidationException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
            { typeof(InfrastructureException), HandleInfrastructureException },
            { typeof(BadRequestException), HandleBadRequestException },
        };
    }

    public override void OnException(ExceptionContext context)
    {
        HandleException(context);
        base.OnException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();
        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }

        if (!context.ModelState.IsValid)
        {
            HandleInvalidModelStateException(context);
            return;
        }

        HandleUnknownException(context);
    }

    private void HandleValidationException(ExceptionContext context)
    {
        var operationResult = new OperationResult
        {
            Success = false
        };
        var exception = (ValidationErrorsException)context.Exception;
        operationResult.InitialException = new IError(HttpStatusCode.UnprocessableEntity.ToString(), (int)HttpStatusCode.UnprocessableEntity, exception.Message);
        operationResult.ValidationErrors = exception.Errors.ToDictionary(x => x.Key, x => x.Value.ToList());
        context.Result = new UnprocessableEntityObjectResult(operationResult);
        context.ExceptionHandled = true;
    }

    private static void HandleInvalidModelStateException(ExceptionContext context)
    {
        context.Result = new UnprocessableEntityObjectResult(new ValidationProblemDetails(context.ModelState));
        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        var operationResult = new OperationResult
        {
            Success = false
        };
        var exception = (NotFoundException)context.Exception;
        operationResult.InitialException = new IError(HttpStatusCode.NotFound.ToString(), (int)HttpStatusCode.NotFound, exception.Message);
        context.Result = new NotFoundObjectResult(operationResult);
        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        var operationResult = new OperationResult
        {
            Success = false
        };
        var exception = (UnauthorizedAccessException)context.Exception;
        operationResult.InitialException = new IError(HttpStatusCode.Unauthorized.ToString(), (int)HttpStatusCode.Unauthorized, exception.Message);
        context.Result = new ObjectResult(operationResult);
        context.ExceptionHandled = true;
    }

    private void HandleForbiddenAccessException(ExceptionContext context)
    {
        var operationResult = new OperationResult
        {
            Success = false
        };
        var exception = (ForbiddenAccessException)context.Exception;
        operationResult.InitialException = new IError(HttpStatusCode.Forbidden.ToString(), (int)HttpStatusCode.Forbidden, exception.Message);
        context.Result = new ObjectResult(operationResult);
        context.ExceptionHandled = true;
    }

    private void HandleInfrastructureException(ExceptionContext context)
    {
        var operationResult = new OperationResult
        {
            Success = false
        };
        var exception = (InfrastructureException)context.Exception;
        operationResult.InitialException = new IError(HttpStatusCode.InternalServerError.ToString(), (int)HttpStatusCode.InternalServerError, exception.Message);
        context.Result = new ObjectResult(operationResult);
        context.ExceptionHandled = true;
    }

    private void HandleBadRequestException(ExceptionContext context)
    {
        var operationResult = new OperationResult
        {
            Success = false
        };
        var exception = (BadRequestException)context.Exception;
        operationResult.InitialException = new IError(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, context.Exception.Message ?? string.Empty);
        context.Result = new BadRequestObjectResult(operationResult);
        context.ExceptionHandled = true;
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Handle Unknown Exception");

        var operationResult = new OperationResult
        {
            Success = false
        };
        operationResult.InitialException = new IError(HttpStatusCode.BadRequest.ToString(), (int)HttpStatusCode.BadRequest, "An unexpected error occurred");
        context.Result = new ObjectResult(operationResult);
        context.ExceptionHandled = true;
    }
}
