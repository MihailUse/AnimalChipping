using Application.Interfaces;
using Application.Models.Animal;
using Application.Models.AnimalType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalModel))]
    public async Task<IActionResult> Get([FromRoute] long animalId)
    {
        return Ok(await _animalService.Get(animalId));
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AnimalModel>))]
    public async Task<IActionResult> Search([FromQuery] AnimalSearchModel searchModel)
    {
        return Ok(await _animalService.Search(searchModel));
    }

    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AnimalModel))]
    public async Task<IActionResult> Create([FromBody] AnimalCreateModel createModel)
    {
        return Ok(await _animalService.Create(createModel));
    }

    [Authorize]
    [HttpPut("{animalId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AnimalModel))]
    public async Task<IActionResult> Update([FromRoute] long animalId, [FromBody] AnimalUpdateModel updateModel)
    {
        return Ok(await _animalService.Update(animalId, updateModel));
    }

    [Authorize]
    [HttpDelete("{animalId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async void Delete([FromRoute] long animalId)
    {
        await _animalService.Delete(animalId);
    }
}