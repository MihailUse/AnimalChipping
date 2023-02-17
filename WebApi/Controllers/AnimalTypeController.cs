using Application.Interfaces;
using Application.Models.Account;
using Application.Models.AnimalType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

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

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalTypeModel))]
    public async Task<IActionResult> Create([FromBody] AnimalTypeCreateModel createModel)
    {
        return Ok(await _animalTypeService.Create(createModel));
    }

    [Authorize]
    [HttpPut("{typeId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalTypeModel))]
    public async Task<IActionResult> Update([FromRoute] long typeId, [FromBody] AnimalTypeUpdateModel updateModel)
    {
        return Ok(await _animalTypeService.Update(typeId, updateModel));
    }

    [Authorize]
    [HttpDelete("{typeId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async void Delete([FromRoute] long typeId)
    {
        await _animalTypeService.Delete(typeId);
    }
}