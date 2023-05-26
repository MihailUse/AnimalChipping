using Application.Interfaces;
using Application.Models.Account;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
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
    public async Task<ActionResult<AccountModel>> Get([FromRoute] int accountId)
    {
        return Ok(await _accountService.Get(accountId));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpGet("search")]
    public async Task<ActionResult<List<AccountModel>>> Search([FromQuery] AccountSearchModel searchModel)
    {
        return Ok(await _accountService.Search(searchModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AccountModel>> Create([FromBody] AccountCreateModel accountCreateModel)
    {
        return StatusCode(StatusCodes.Status201Created, await _accountService.Create(accountCreateModel));
    }

    [HttpPut("{accountId:int}")]
    public async Task<ActionResult<AccountModel>> Update(
        [FromRoute] int accountId,
        [FromBody] AccountUpdateModel updateModel
    )
    {
        return Ok(await _accountService.Update(accountId, updateModel));
    }

    [HttpDelete("{accountId:int}")]
    public async Task<ActionResult> Delete([FromRoute] int accountId)
    {
        await _accountService.Delete(accountId);
        return Ok();
    }
}