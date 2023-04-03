using Application.Interfaces;
using Application.Models.Account;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

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

    [Unauthorized]
    [HttpPost("registration")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Registration([FromBody] AccountRegistrationModel accountCreateModel)
    {
        return StatusCode(StatusCodes.Status201Created, await _accountService.Registrate(accountCreateModel));
    }
}