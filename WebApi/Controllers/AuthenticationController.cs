using Application.Interfaces;
using Application.Models.Account;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("/")]
public class AuthenticationController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AuthenticationController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("registration")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Registration([FromBody] AccountCreateModel accountCreateModel)
    {
        return Ok(await _accountService.Create(accountCreateModel));
    }
}