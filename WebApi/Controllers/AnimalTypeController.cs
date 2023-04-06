using Application.Entities;
using Application.Interfaces;
using Application.Models.AnimalType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Controllers;

[Authorize]
[ValidateIdentifier]
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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalTypeModel))]
    public async Task<IActionResult> Get([FromRoute] long typeId)
    {
        return Ok(await _animalTypeService.Get(typeId));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalTypeModel))]
    public async Task<IActionResult> Create([FromBody] AnimalTypeCreateModel model)
    {
        return StatusCode(StatusCodes.Status201Created, await _animalTypeService.Create(model));
    }

    [CheckRole(AccountRole.ADMIN | AccountRole.CHIPPER)]
    [HttpPut("{typeId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalTypeModel))]
    public async Task<IActionResult> Update([FromRoute] long typeId, [FromBody] AnimalTypeUpdateModel updateModel)
    {
        return Ok(await _animalTypeService.Update(typeId, updateModel));
    }

    [CheckRole(AccountRole.ADMIN)]
    [HttpDelete("{typeId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task Delete([FromRoute] long typeId)
    {
        await _animalTypeService.Delete(typeId);
    }
}