using Application.Interfaces;
using Application.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    public async Task<IActionResult> Get([FromRoute] int accountId)
    {
        return Ok(await _accountService.Get(accountId));
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountModel>))]
    public async Task<IActionResult> Search([FromQuery] AccountSearchModel searchModel)
    {
        return Ok(await _accountService.Search(searchModel));
    }

    [Authorize]
    [HttpPut("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    public async Task<IActionResult> Update([FromRoute] int accountId, [FromBody] AccountUpdateModel updateModel)
    {
        return Ok(await _accountService.Update(accountId, updateModel));
    }

    [Authorize]
    [HttpDelete("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async void Delete([FromRoute] int accountId)
    {
        await _accountService.Delete(accountId);
    }
}