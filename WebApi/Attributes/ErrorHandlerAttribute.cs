using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApi.Attributes;

public class ErrorHandlerAttribute : ExceptionFilterAttribute
{
    private readonly ILogger<ErrorHandlerAttribute> _logger;

    public ErrorHandlerAttribute(ILogger<ErrorHandlerAttribute> logger)
    {
        _logger = logger;
    }

    public override void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var result = exception switch
        {
            BadOperationException => new BadRequestObjectResult(exception.Message),
            UnauthorizedException => new UnauthorizedObjectResult(exception.Message),
            ForbiddenException => new ObjectResult(exception.Message) { StatusCode = StatusCodes.Status403Forbidden },
            NotFoundException => new NotFoundObjectResult(exception.Message),
            ConflictException => new ConflictObjectResult(exception.Message),
            _ => new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError }
        };

        if (result.StatusCode == StatusCodes.Status500InternalServerError)
            _logger.Log(
                LogLevel.Error,
                "Request {Method} {Url} Error: {Error}",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path.Value,
                exception.Message
            );

        context.Result = result;
    }
}