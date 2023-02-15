using Application.Interfaces;
using Application.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Authorize]
[Route("accounts")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    public async Task<IActionResult> GetById([FromRoute] int accountId)
    {
        return Ok(await _accountService.GetAccount(accountId));
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountModel>))]
    public async Task<IActionResult> Search([FromQuery] AccountSearchModel searchModel)
    {
        return Ok(await _accountService.Search(searchModel));
    }

    [HttpPut("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    public async Task<IActionResult> Update([FromRoute] int accountId, [FromBody] AccountUpdateModel updateModel)
    {
        return Ok(await _accountService.Update(accountId, updateModel));
    }
}