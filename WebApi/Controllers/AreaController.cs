using Application.Entities;
using Application.Interfaces;
using Application.Models.Area;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
[ValidateIdentifier]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AreaModel))]
    public async Task<IActionResult> Get([FromRoute] long areaId)
    {
        return Ok(await _areaService.Get(areaId));
    }

    [HttpGet("{areaId:long}/analytics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnalyticModel))]
    public async Task<IActionResult> GetAnalytic([FromRoute] long areaId, [FromQuery] GetAnalyticModel model)
    {
        return Ok(await _areaService.GetAnalytic(areaId, model));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AreaModel))]
    public async Task<IActionResult> Create([FromBody] AreaCreateModel createModel)
    {
        return StatusCode(StatusCodes.Status201Created, await _areaService.Create(createModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpPut("{areaId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AreaModel))]
    public async Task<IActionResult> Update([FromRoute] long areaId, [FromBody] AreaUpdateModel updateModel)
    {
        return Ok(await _areaService.Update(areaId, updateModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpDelete("{areaId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task Delete([FromRoute] long areaId)
    {
        await _areaService.Delete(areaId);
    }
}