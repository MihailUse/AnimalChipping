using Application.Interfaces;
using Application.Models.Animal;
using Application.Models.AnimalVisitedLocation;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("animals")]
public class AnimalController : ControllerBase
{
    private readonly IAnimalService _animalService;

    public AnimalController(IAnimalService animalService)
    {
        _animalService = animalService;
    }

    [HttpGet("{animalId:long}")]
    public async Task<ActionResult<AnimalModel>> Get([FromRoute] long animalId)
    {
        return Ok(await _animalService.Get(animalId));
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<AnimalModel>>> Search([FromQuery] AnimalSearchModel searchModel)
    {
        return Ok(await _animalService.Search(searchModel));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AnimalModel>> Create([FromBody] AnimalCreateModel createModel)
    {
        return StatusCode(StatusCodes.Status201Created, await _animalService.Create(createModel));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPut("{animalId:long}")]
    public async Task<ActionResult<AnimalModel>> Update(
        [FromRoute] long animalId,
        [FromBody] AnimalUpdateModel updateModel
    )
    {
        return Ok(await _animalService.Update(animalId, updateModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpDelete("{animalId:long}")]
    public async Task<ActionResult> Delete([FromRoute] long animalId)
    {
        await _animalService.Delete(animalId);
        return Ok();
    }

    #region AnimalType

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPost("{animalId:long}/types/{typeId:long}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AnimalModel>> AddType([FromRoute] long animalId, [FromRoute] long typeId)
    {
        return StatusCode(StatusCodes.Status201Created, await _animalService.AddType(animalId, typeId));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPut("{animalId:long}/types")]
    public async Task<ActionResult<AnimalModel>> UpdateType(
        [FromRoute] long animalId,
        [FromBody] AnimalUpdateTypeModel updateTypeModel
    )
    {
        return Ok(await _animalService.UpdateType(animalId, updateTypeModel));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpDelete("{animalId:long}/types/{typeId:long}")]
    public async Task<ActionResult<AnimalModel>> DeleteType([FromRoute] long animalId, [FromRoute] long typeId)
    {
        return Ok(await _animalService.DeleteType(animalId, typeId));
    }

    #endregion

    #region AnimalLocation

    [HttpGet("{animalId:long}/locations")]
    public async Task<ActionResult<List<AnimalVisitedLocationModel>>> SearchLocation(
        [FromRoute] long animalId,
        [FromQuery] AnimalSearchLocationModel searchLocationModel
    )
    {
        return Ok(await _animalService.SearchLocation(animalId, searchLocationModel));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPost("{animalId:long}/locations/{pointId:long}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AnimalVisitedLocationModel>> AddLocation(
        [FromRoute] long animalId,
        [FromRoute] long pointId
    )
    {
        return StatusCode(StatusCodes.Status201Created, await _animalService.AddLocation(animalId, pointId));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPut("{animalId:long}/locations")]
    public async Task<ActionResult<AnimalVisitedLocationModel>> UpdateLocation(
        [FromRoute] long animalId,
        [FromBody] AnimalUpdateLocationModel updateLocationModel
    )
    {
        return Ok(await _animalService.UpdateLocation(animalId, updateLocationModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpDelete("{animalId:long}/locations/{visitedPointId:long}")]
    public async Task<ActionResult> DeleteLocation([FromRoute] long animalId, [FromRoute] long visitedPointId)
    {
        await _animalService.DeleteLocation(animalId, visitedPointId);
        return Ok();
    }

    #endregion
}