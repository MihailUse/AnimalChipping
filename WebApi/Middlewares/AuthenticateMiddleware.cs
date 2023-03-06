using System.Net.Http.Headers;
using System.Text;
using Application.Exceptions;
using Application.Interfaces;

namespace WebApi.Middlewares;

public class AuthenticateMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticateMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, IAccountService accountService)
    {
        var hasHeader = AuthenticationHeaderValue.TryParse(httpContext.Request.Headers["Authorization"], out var authHeader);
        if (hasHeader && authHeader!.Parameter != default)
        {
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var email = credentials[0];
            var password = credentials[1];

            var accountExists = await accountService.CheckExists(email, password);
            if (!accountExists)
                throw new AccessDenied("Invalid credentials");
        }

        await _next(httpContext);
    }
}

public static class AuthenticateMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticateMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticateMiddleware>();
    }
}