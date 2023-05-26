using Application.Interfaces;
using Application.Models.AnimalType;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route("animals/types")]
public class AnimalTypeController : ControllerBase
{
    private readonly IAnimalTypeService _animalTypeService;

    public AnimalTypeController(IAnimalTypeService animalTypeService)
    {
        _animalTypeService = animalTypeService;
    }

    [HttpGet("{typeId:long}")]
    public async Task<ActionResult<AnimalTypeModel>> Get([FromRoute] long typeId)
    {
        return Ok(await _animalTypeService.Get(typeId));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<AnimalTypeModel>> Create([FromBody] AnimalTypeCreateModel model)
    {
        return StatusCode(StatusCodes.Status201Created, await _animalTypeService.Create(model));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPut("{typeId:long}")]
    public async Task<ActionResult<AnimalTypeModel>> Update(
        [FromRoute] long typeId,
        [FromBody] AnimalTypeUpdateModel updateModel
    )
    {
        return Ok(await _animalTypeService.Update(typeId, updateModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpDelete("{typeId:long}")]
    public async Task<ActionResult> Delete([FromRoute] long typeId)
    {
        await _animalTypeService.Delete(typeId);
        return Ok();
    }
}