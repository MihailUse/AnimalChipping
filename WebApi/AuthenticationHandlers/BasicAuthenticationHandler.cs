using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WebApi.AuthenticationHandlers;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ICurrentAccount _currentAccount;
    private readonly IAccountService _accountService;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ICurrentAccount currentAccount,
        IAccountService accountService
    ) : base(options, logger, encoder, clock)
    {
        _currentAccount = currentAccount;
        _accountService = accountService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader.Parameter == default)
                return AuthenticateResult.Fail("Invalid Credentials");

            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var email = credentials[0];
            var password = credentials[1];
            
            _currentAccount.Account = await _accountService.Authenticate(email, password);
        }
        catch
        {
            return AuthenticateResult.Fail("Error Occured.Authorization failed.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.PrimarySid, _currentAccount.Account.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, _currentAccount.Account.Email),
            new Claim(ClaimTypes.Name, _currentAccount.Account.FirstName),
            new Claim(ClaimTypes.Surname, _currentAccount.Account.LastName),
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}