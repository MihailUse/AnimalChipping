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

    [AllowAnonymous]
    [HttpGet("{typeId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalTypeModel))]
    public async Task<IActionResult> Get([FromRoute] long typeId)
    {
        return Ok(await _animalTypeService.Get(typeId));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalTypeModel))]
    public async Task<IActionResult> Create([FromBody] AnimalTypeCreateModel model)
    {
        return StatusCode(StatusCodes.Status201Created, await _animalTypeService.Create(model));
    }

    [HttpPut("{typeId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalTypeModel))]
    public async Task<IActionResult> Update([FromRoute] long typeId, [FromBody] AnimalTypeUpdateModel updateModel)
    {
        return Ok(await _animalTypeService.Update(typeId, updateModel));
    }

    [HttpDelete("{typeId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task Delete([FromRoute] long typeId)
    {
        await _animalTypeService.Delete(typeId);
    }
}