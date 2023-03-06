using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<ErrorHandlerMiddleware>();
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            ObjectResult result = e switch
            {
                InvalidOperationException => new BadRequestObjectResult(e.Message),
                AccessDenied => new UnauthorizedObjectResult(e.Message) { StatusCode = StatusCodes.Status403Forbidden },
                NotFoundException => new NotFoundObjectResult(e.Message),
                ConflictException => new ConflictObjectResult(e.Message),
                _ => new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError }
            };

            if (result.StatusCode == StatusCodes.Status500InternalServerError)
                _logger.Log(
                    LogLevel.Error,
                    "Request {Method} {Url} Error: {Error}",
                    httpContext.Request.Method,
                    httpContext.Request.Path.Value,
                    e.Message
                );

            await result.ExecuteResultAsync(new ActionContext
            {
                HttpContext = httpContext
            });
        }
    }
}

public static class ErrorHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlerMiddleware>();
    }
}