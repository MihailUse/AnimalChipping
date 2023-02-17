using Application.Interfaces;
using Application.Models.Location;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("{locationId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationPointModel))]
    public async Task<IActionResult> Get([FromRoute] int locationId)
    {
        return Ok(await _locationService.Get(locationId));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(LocationPointModel))]
    public async Task<IActionResult> Create([FromBody] LocationPointCreateModel locationPointCreateModel)
    {
        return Ok(await _locationService.Create(locationPointCreateModel));
    }

    [Authorize]
    [HttpPut("{pointId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationPointModel))]
    public async Task<IActionResult> Update([FromRoute] long pointId, [FromBody] LocationPointUpdateModel updateModel)
    {
        return Ok(await _locationService.Update(pointId, updateModel));
    }

    [Authorize]
    [HttpDelete("{pointId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async void Delete([FromRoute] long pointId)
    {
        await _locationService.Delete(pointId);
    }
}