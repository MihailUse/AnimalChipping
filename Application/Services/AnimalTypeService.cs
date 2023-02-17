using Application.Exceptions;
using Application.Interfaces;
using Application.Models.AnimalType;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

internal class AnimalTypeService : IAnimalTypeService
{
    private readonly IMapper _mapper;
    private readonly IDatabaseContext _context;

    public AnimalTypeService(IMapper mapper, IDatabaseContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<AnimalTypeModel> Get(long typeId)
    {
        var type = await _context.AnimalTypes.FindAsync(typeId);
        if (type == default)
            throw new NotFoundException();

        return _mapper.Map<AnimalTypeModel>(type);
    }

    public async Task<AnimalTypeModel> Create(AnimalTypeCreateModel createModel)
    {
        var typeExists = await _context.AnimalTypes.AnyAsync(x => x.Type == createModel.Type);
        if (typeExists == default)
            throw new ConflictException();

        var type = _mapper.Map<AnimalType>(createModel);
        await _context.AnimalTypes.AddAsync(type);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalTypeModel>(type);
    }

    public async Task<AnimalTypeModel> Update(long typeId, AnimalTypeUpdateModel updateModel)
    {
        var existsType = await _context.AnimalTypes.FindAsync(typeId);
        if (existsType == default)
            throw new NotFoundException();

        var type = _mapper.Map(updateModel, existsType);
        type.Id = typeId;

        _context.AnimalTypes.Update(type);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalTypeModel>(type);
    }

    public async Task Delete(long typeId)
    {
        var type = await _context.AnimalTypes.FindAsync(typeId);
        if (type == default)
            throw new NotFoundException();

        var hasAnimals = await _context.AnimalTypes.AnyAsync(x => x.Animals.Any());
        if (hasAnimals)
            throw new InvalidOperationException();

        _context.AnimalTypes.Remove(type);
        await _context.SaveChangesAsync();
    }
}