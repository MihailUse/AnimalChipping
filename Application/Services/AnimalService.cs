using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Animal;
using Application.Models.AnimalVisitedLocation;
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
        var animal = await FindAnimal(animalId);
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
            throw new ConflictException("AnimalTypes has duplicates");

        var existsAnimalTypes = await _context.AnimalTypes
            .Where(x => createModel.AnimalTypes.Contains(x.Id))
            .ToListAsync();
        if (existsAnimalTypes.Count != createModel.AnimalTypes.Count)
            throw new NotFoundException("AnimalType not found");

        // check account exists and location exists
        var accountExists = await _context.Accounts.AnyAsync(x => x.Id == createModel.ChipperId);
        if (accountExists == default)
            throw new NotFoundException("Account not found");

        var locationExists = await _context.LocationPoints.AnyAsync(x => x.Id == createModel.ChippingLocationId);
        if (locationExists == default)
            throw new NotFoundException("Location not found");

        // map
        var animal = _mapper.Map<Animal>(createModel);
        animal.AnimalTypes = existsAnimalTypes;

        await _context.Animals.AddAsync(animal);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalModel>(animal);
    }

    public async Task<AnimalModel> Update(long animalId, AnimalUpdateModel updateModel)
    {
        var animal = await FindAnimal(animalId);

        if (animal.ChipperId != updateModel.ChipperId)
        {
            var accountExists = await _context.Accounts.AnyAsync(x => x.Id == updateModel.ChipperId);
            if (accountExists == default)
                throw new NotFoundException("Account not found");
        }

        if (animal.ChippingLocationId != updateModel.ChippingLocationId)
        {
            var locationExists = await _context.LocationPoints.AnyAsync(x => x.Id == updateModel.ChippingLocationId);
            if (locationExists == default)
                throw new NotFoundException("Location not found");
        }

        animal = _mapper.Map(updateModel, animal);

        _context.Animals.Update(animal);
        await _context.SaveChangesAsync();

        return await _context.Animals
            .ProjectTo<AnimalModel>(_mapper.ConfigurationProvider)
            .FirstAsync(x => x.Id == animalId);
    }

    public async Task Delete(long animalId)
    {
        var animal = await FindAnimal(animalId);
        _context.Animals.Remove(animal);
        await _context.SaveChangesAsync();
    }

    #region AnimalType

    public async Task<AnimalModel> AddType(long animalId, long typeId)
    {
        var animal = await FindFullAnimal(animalId);

        var animalType = await _context.AnimalTypes.FindAsync(typeId);
        if (animalType == default)
            throw new NotFoundException("AnimalType not found");

        if (animal.AnimalTypes.Contains(animalType))
            throw new ConflictException("Animal already contains type");

        animal.AnimalTypes.Add(animalType);
        _context.Animals.Update(animal);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalModel>(animal);
    }

    public async Task<AnimalModel> UpdateType(long animalId, AnimalUpdateTypeModel updateTypeModel)
    {
        var animal = await FindFullAnimal(animalId);

        var removedTypes = animal.AnimalTypes.RemoveAll(x => x.Id == updateTypeModel.OldTypeId);
        if (removedTypes == 0)
            throw new NotFoundException("Old type not found");

        var typeAlreadyExists = animal.AnimalTypes.Any(x => x.Id == updateTypeModel.NewTypeId);
        if (typeAlreadyExists)
            throw new ConflictException("New type already exists");

        var newAnimalType = await _context.AnimalTypes.FindAsync(updateTypeModel.NewTypeId);
        if (newAnimalType == default)
            throw new NotFoundException("New Type not found");

        animal.AnimalTypes.Add(newAnimalType);
        _context.Animals.Update(animal);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalModel>(animal);
    }

    public async Task<AnimalModel> DeleteType(long animalId, long typeId)
    {
        var animal = await FindFullAnimal(animalId);

        var removedTypes = animal.AnimalTypes.RemoveAll(x => x.Id == typeId);
        if (removedTypes == 0)
            throw new NotFoundException("AnimalType not found");

        _context.Animals.Remove(animal);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalModel>(animal);
    }

    #endregion

    #region AnimalLocation

    public async Task<List<AnimalVisitedLocationModel>> SearchLocation(
        long animalId,
        AnimalSearchLocationModel searchLocationModel
    )
    {
        var query = _context.AnimalVisitedLocations.Where(x => x.AnimalId == animalId);

        if (searchLocationModel.StartDateTime != default)
            query = query.Where(x => x.DateTimeOfVisitLocationPoint < searchLocationModel.StartDateTime);

        if (searchLocationModel.EndDateTime != default)
            query = query.Where(x => x.DateTimeOfVisitLocationPoint > searchLocationModel.EndDateTime);

        return await query
            .OrderByDescending(x => x.DateTimeOfVisitLocationPoint)
            .Skip(searchLocationModel.From)
            .Take(searchLocationModel.Size)
            .ProjectTo<AnimalVisitedLocationModel>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<AnimalVisitedLocationModel> AddLocation(long animalId, long pointId)
    {
        var animal = await _context.Animals
            .Include(x => x.VisitedLocations)
            .FirstOrDefaultAsync(x => x.Id == animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");

        if (animal.LifeStatus == Animal.AnimalLifeStatus.DEAD)
            throw new InvalidOperationException();

        var location = await _context.LocationPoints.FindAsync(pointId);
        if (location == default)
            throw new NotFoundException("Location not found");

        var lastVisitedLocation = animal.VisitedLocations
            .OrderByDescending(x => x.DateTimeOfVisitLocationPoint)
            .First();
        if (lastVisitedLocation.LocationPointId == pointId)
            throw new InvalidOperationException("Point already exists");

        var animalVisitedLocation = new AnimalVisitedLocation()
        {
            Animal = animal,
            LocationPoint = location
        };

        await _context.AnimalVisitedLocations.AddAsync(animalVisitedLocation);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalVisitedLocationModel>(animalVisitedLocation);
    }

    public async Task<AnimalVisitedLocationModel> UpdateLocation(
        long animalId,
        AnimalUpdateLocationModel updateLocationModel
    )
    {
        var animal = await _context.Animals
            .Include(x => x.VisitedLocations)
            .FirstOrDefaultAsync(x => x.Id == animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");

        var location = await _context.LocationPoints.FindAsync(updateLocationModel.LocationPointId);
        if (location == default)
            throw new NotFoundException("Location not found");

        var oldAnimalVisitedLocation = animal.VisitedLocations
            .Find(x => x.Id == updateLocationModel.VisitedLocationPointId);
        if (oldAnimalVisitedLocation == default)
            throw new NotFoundException("Old visited location not found");

        var animalVisitedLocation = new AnimalVisitedLocation()
        {
            Animal = animal,
            LocationPoint = location
        };

        _context.AnimalVisitedLocations.Remove(oldAnimalVisitedLocation);
        await _context.AnimalVisitedLocations.AddAsync(animalVisitedLocation);
        await _context.SaveChangesAsync();

        return _mapper.Map<AnimalVisitedLocationModel>(animalVisitedLocation);
    }

    public async Task DeleteLocation(long animalId, long visitedPointId)
    {
        var animalVisitedLocation = await _context.AnimalVisitedLocations.FindAsync(visitedPointId);
        if (animalVisitedLocation == default)
            throw new NotFoundException("Visited location not found");

        _context.AnimalVisitedLocations.Remove(animalVisitedLocation);
        await _context.SaveChangesAsync();
    }

    #endregion

    private async Task<Animal> FindFullAnimal(long animalId)
    {
        var animal = await _context.Animals
            .Include(x => x.AnimalTypes)
            .Include(x => x.VisitedLocations)
            .FirstOrDefaultAsync(x => x.Id == animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");

        return animal;
    }

    private async Task<Animal> FindAnimal(long animalId)
    {
        var animal = await _context.Animals.FindAsync(animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");

        return animal;
    }
}