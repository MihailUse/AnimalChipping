using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Middlewares;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception e)
        {
            var result = e switch
            {
                AccessDenied => new StatusCodeResult(StatusCodes.Status403Forbidden),
                NotFoundException => new StatusCodeResult(StatusCodes.Status404NotFound),
                ConflictException => new StatusCodeResult(StatusCodes.Status409Conflict),
                _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
            };

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