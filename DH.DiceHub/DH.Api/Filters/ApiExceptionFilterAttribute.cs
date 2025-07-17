using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using DH.OperationResultCore.Exceptions;

namespace DH.Api.Filters;

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

        _logger.LogError(context.Exception, "Exception occurred: {ExceptionType}", type.Name);

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
        var exception = (ValidationErrorsException)context.Exception;
        _logger.LogWarning("Validation error occurred:\n{ValidationDetails}", exception.ToString());

        context.Result = new UnprocessableEntityObjectResult(new ValidationProblemDetails(exception.Errors));
        context.ExceptionHandled = true;
    }

    private static void HandleInvalidModelStateException(ExceptionContext context)
    {
        context.Result = new UnprocessableEntityObjectResult(new ValidationProblemDetails(context.ModelState));
        context.ExceptionHandled = true;
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        _logger.LogWarning("Resource not found: {Message}", context.Exception.Message);

        context.Result = new NotFoundObjectResult(new ProblemDetails()
        {
            Title = "The specified resource was not found.",
            Detail = context.Exception.Message
        });
        context.ExceptionHandled = true;
    }

    private void HandleUnauthorizedAccessException(ExceptionContext context)
    {
        _logger.LogWarning("Unauthorized access: {Message}", context.Exception.Message);

        var details = new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Unauthorized"
        };
        context.Result = new ObjectResult(details)
        {
            StatusCode = details.Status
        };
        context.ExceptionHandled = true;
    }

    private void HandleForbiddenAccessException(ExceptionContext context)
    {
        _logger.LogWarning("Forbidden access: {Message}", context.Exception.Message);

        var details = new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = context.Exception.Message
        };
        context.Result = new ObjectResult(details)
        {
            StatusCode = details.Status
        };
        context.ExceptionHandled = true;
    }

    private void HandleInfrastructureException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Infrastructure exception occurred");

        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while interacting with another application.",
            Detail = context.Exception.Message
        };
        context.Result = new ObjectResult(details)
        {
            StatusCode = details.Status
        };
        context.ExceptionHandled = true;
    }

    private void HandleBadRequestException(ExceptionContext context)
    {
        _logger.LogWarning("Bad request: {Message}", context.Exception.Message);

        context.Result = new BadRequestObjectResult(new ProblemDetails
        {
            Detail = context.Exception.Message
        });
        context.ExceptionHandled = true;
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception occurred");

        var details = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = "An unexpected error occurred"
        };
        context.Result = new ObjectResult(details)
        {
            StatusCode = details.Status
        };
        context.ExceptionHandled = true;
    }
}
