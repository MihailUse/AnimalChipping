using Application.Interfaces;
using Application.Models.Area;
using Application.Models.Location;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("locations")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<string>> GetIdByPoint([FromQuery] PointModel model)
    {
        return Ok(await _locationService.GetIdByPoint(model));
    }

    [HttpGet("{locationId:long}")]
    public async Task<ActionResult<LocationPointModel>> Get([FromRoute] int locationId)
    {
        return Ok(await _locationService.Get(locationId));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<LocationPointModel>> Create(
        [FromBody] LocationPointCreateModel locationPointCreateModel
    )
    {
        return StatusCode(StatusCodes.Status201Created, await _locationService.Create(locationPointCreateModel));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPut("{pointId:long}")]
    public async Task<ActionResult<LocationPointModel>> Update(
        [FromRoute] long pointId,
        [FromBody] LocationPointUpdateModel updateModel
    )
    {
        return Ok(await _locationService.Update(pointId, updateModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpDelete("{pointId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Delete([FromRoute] long pointId)
    {
        await _locationService.Delete(pointId);
        return Ok();
    }

    [HttpGet("geohash")]
    public async Task<ActionResult<string>> GetOpenLocationCode([FromQuery] PointModel model)
    {
        return Ok(await _locationService.GetOpenLocationCode(model));
    }

    [HttpGet("geohashv2")]
    public async Task<ActionResult<string>> GetOpenLocationCodeBase64([FromQuery] PointModel model)
    {
        return Ok(await _locationService.GetOpenLocationCodeBase64(model));
    }

    [HttpGet("geohashv3")]
    public async Task<ActionResult<string>> GetOpenLocationCodeHash([FromQuery] PointModel model)
    {
        return Ok(await _locationService.GetOpenLocationCodeHash(model));
    }
}