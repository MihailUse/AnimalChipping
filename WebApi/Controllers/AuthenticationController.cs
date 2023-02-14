using Application.Interfaces;
using Application.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("[action]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AuthenticationController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Registration([FromBody] CreateAccountModel createAccountModel)
    {
        return Ok(await _accountService.CreateAccount(createAccountModel));
    }
}