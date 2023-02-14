using Application.Interfaces;
using Application.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("accounts")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    public async Task<IActionResult> Get(Guid accountId)
    {
        return Ok(await _accountService.GetAccount(accountId));
    }
}