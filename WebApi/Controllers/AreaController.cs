using Application.Interfaces;
using Application.Models.Area;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("areas")]
public class AreaController : ControllerBase
{
    private readonly IAreaService _areaService;

    public AreaController(IAreaService areaService)
    {
        _areaService = areaService;
    }

    [HttpGet("{areaId:long}")]
    public async Task<ActionResult<AreaModel>> Get([FromRoute] long areaId)
    {
        return Ok(await _areaService.Get(areaId));
    }

    [HttpGet("{areaId:long}/analytics")]
    public async Task<ActionResult<AnalyticModel>> GetAnalytic(
        [FromRoute] long areaId,
        [FromQuery] GetAnalyticModel model
    )
    {
        return Ok(await _areaService.GetAnalytic(areaId, model));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AreaModel>> Create([FromBody] AreaCreateModel createModel)
    {
        return StatusCode(StatusCodes.Status201Created, await _areaService.Create(createModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpPut("{areaId:long}")]
    public async Task<ActionResult<AreaModel>> Update(
        [FromRoute] long areaId,
        [FromBody] AreaUpdateModel updateModel
    )
    {
        return Ok(await _areaService.Update(areaId, updateModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpDelete("{areaId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete([FromRoute] long areaId)
    {
        await _areaService.Delete(areaId);
        return Ok();
    }
}