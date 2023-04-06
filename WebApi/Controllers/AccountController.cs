using Application.Entities;
using Application.Interfaces;
using Application.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
[ValidateIdentifier]
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

    [CheckRole(AccountRole.ADMIN)]
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AccountModel>))]
    public async Task<IActionResult> Search([FromQuery] AccountSearchModel searchModel)
    {
        return Ok(await _accountService.Search(searchModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] AccountCreateModel accountCreateModel)
    {
        return StatusCode(StatusCodes.Status201Created, await _accountService.Create(accountCreateModel));
    }

    [HttpPut("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountModel))]
    public async Task<IActionResult> Update([FromRoute] int accountId, [FromBody] AccountUpdateModel updateModel)
    {
        return Ok(await _accountService.Update(accountId, updateModel));
    }

    [HttpDelete("{accountId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task Delete([FromRoute] int accountId)
    {
        await _accountService.Delete(accountId);
    }
}