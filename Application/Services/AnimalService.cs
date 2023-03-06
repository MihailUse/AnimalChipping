using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Animal;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

internal class AnimalService : IAnimalService
{
    private readonly IMapper _mapper;
    private readonly IDatabaseContext _context;

    public AnimalService(IMapper mapper, IDatabaseContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<AnimalModel> Get(long animalId)
    {
        var animal = await _context.Animals.FindAsync(animalId);
        if (animal == default)
            throw new NotFoundException();

        return _mapper.Map<AnimalModel>(animal);
    }

    public async Task<List<AnimalModel>> Search(AnimalSearchModel searchModel)
    {
        IQueryable<Animal> query = _context.Animals;

        if (searchModel.ChipperId != default)
            query = query.Where(x => x.ChipperId == searchModel.ChipperId);

        if (searchModel.ChippingLocationId != default)
            query = query.Where(x => x.ChippingLocationId == searchModel.ChippingLocationId);

        if (searchModel.Gender != default)
            query = query.Where(x => x.Gender == Enum.Parse<Animal.AnimalGender>(searchModel.Gender));

        if (searchModel.LifeStatus != default)
            query = query.Where(x => x.LifeStatus == Enum.Parse<Animal.AnimalLifeStatus>(searchModel.LifeStatus));

        if (searchModel.StartDateTime != default)
            query = query.Where(x => x.ChippingDateTime > searchModel.StartDateTime);

        if (searchModel.EndDateTime != default)
            query = query.Where(x => x.ChippingDateTime < searchModel.EndDateTime);

        return await query
            .OrderBy(x => x.Id)
            .Skip(searchModel.From)
            .Take(searchModel.Size)
            .ProjectTo<AnimalModel>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<AnimalModel> Create(AnimalCreateModel createModel)
    {
        // check animal types
        var hasDuplicates = createModel.AnimalTypes.Distinct().Count() != createModel.AnimalTypes.Count;
        if (hasDuplicates)
            throw new ConflictException();

        var existsAnimalTypes = await _context.AnimalTypes
            .Where(x => createModel.AnimalTypes.Contains(x.Id))
            .ToListAsync();
        if (existsAnimalTypes.Count != createModel.AnimalTypes.Count)
            throw new NotFoundException();

        // check account exists and location exists
        var accountExists = await _context.Accounts.AnyAsync(x => x.Id == createModel.ChipperId);
        var locationExists = await _context.LocationPoints.AnyAsync(x => x.Id == createModel.ChippingLocationId);
        if (accountExists == default || locationExists == default)
            throw new NotFoundException();

        // map
        var animal = _mapper.Map<Animal>(createModel);
        animal.AnimalTypes = existsAnimalTypes;

        await _context.Animals.AddAsync(animal);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalModel>(animal);
    }

    public async Task<AnimalModel> Update(long animalId, AnimalUpdateModel updateModel)
    {
        var animal = await _context.Animals.FindAsync(animalId);
        if (animal == default)
            throw new NotFoundException();

        // check account exists and location exists
        var accountExists = await _context.Accounts.AnyAsync(x => x.Id == updateModel.ChipperId);
        var locationExists = await _context.LocationPoints.AnyAsync(x => x.Id == updateModel.ChippingLocationId);
        if (accountExists == default || locationExists == default)
            throw new NotFoundException();

        animal = _mapper.Map<Animal>(updateModel);

        _context.Animals.Update(animal);
        await _context.SaveChangesAsync();

        return await _context.Animals
            .ProjectTo<AnimalModel>(_mapper.ConfigurationProvider)
            .FirstAsync(x => x.Id == animalId);
    }

    public Task Delete(long animalId)
    {
        throw new NotImplementedException();
    }
}