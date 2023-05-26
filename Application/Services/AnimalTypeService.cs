using Application.Exceptions;
using Application.Interfaces;
using Application.Models.AnimalType;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

internal class AnimalTypeService : IAnimalTypeService
{
    private readonly IMapper _mapper;
    private readonly IDatabaseContext _database;

    public AnimalTypeService(IMapper mapper, IDatabaseContext database)
    {
        _mapper = mapper;
        _database = database;
    }

    public async Task<AnimalTypeModel> Get(long typeId)
    {
        var type = await _database.AnimalTypes.FindAsync(typeId);
        if (type == default)
            throw new NotFoundException("Type not found");

        return _mapper.Map<AnimalTypeModel>(type);
    }

    public async Task<AnimalTypeModel> Create(AnimalTypeCreateModel model)
    {
        var typeExists = await _database.AnimalTypes.AnyAsync(x => x.Type == model.Type);
        if (typeExists != default)
            throw new ConflictException("Type already exists");

        var type = _mapper.Map<AnimalType>(model);
        await _database.AnimalTypes.AddAsync(type);
        await _database.SaveChangesAsync();

        return _mapper.Map<AnimalTypeModel>(type);
    }

    public async Task<AnimalTypeModel> Update(long typeId, AnimalTypeUpdateModel updateModel)
    {
        var type = await _database.AnimalTypes.FindAsync(typeId);
        if (type == default)
            throw new NotFoundException("Type not found");

        var newNameExists = await _database.AnimalTypes.AnyAsync(x => x.Id != typeId && x.Type == updateModel.Type);
        if (newNameExists)
            throw new ConflictException("Type already exists");

        type = _mapper.Map(updateModel, type);
        _database.AnimalTypes.Update(type);
        await _database.SaveChangesAsync();

        return _mapper.Map<AnimalTypeModel>(type);
    }

    public async Task Delete(long typeId)
    {
        var type = await _database.AnimalTypes.FindAsync(typeId);
        if (type == default)
            throw new NotFoundException("Type not found");

        var hasAnimals = await _database.Animals.AnyAsync(x => x.AnimalTypes.Any(t => t.Id == typeId));
        if (hasAnimals)
            throw new BadOperationException("Type contains animals");

        _database.AnimalTypes.Remove(type);
        await _database.SaveChangesAsync();
    }
}