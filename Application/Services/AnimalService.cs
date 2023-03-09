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
    private readonly IDatabaseContext _database;

    public AnimalService(IMapper mapper, IDatabaseContext database)
    {
        _mapper = mapper;
        _database = database;
    }

    public async Task<AnimalModel> Get(long animalId)
    {
        var animal = await FindFullAnimal(animalId);
        return _mapper.Map<AnimalModel>(animal);
    }

    public async Task<List<AnimalModel>> Search(AnimalSearchModel searchModel)
    {
        IQueryable<Animal> query = _database.Animals;

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

        var existsAnimalTypes = await _database.AnimalTypes
            .Where(x => createModel.AnimalTypes.Contains(x.Id))
            .ToListAsync();
        if (existsAnimalTypes.Count != createModel.AnimalTypes.Count)
            throw new NotFoundException("AnimalType not found");

        // check account exists and location exists
        var accountExists = await _database.Accounts.AnyAsync(x => x.Id == createModel.ChipperId);
        if (accountExists == default)
            throw new NotFoundException("Account not found");

        var locationExists = await _database.LocationPoints.AnyAsync(x => x.Id == createModel.ChippingLocationId);
        if (locationExists == default)
            throw new NotFoundException("Location not found");

        // map
        var animal = _mapper.Map<Animal>(createModel);
        animal.AnimalTypes = existsAnimalTypes;

        await _database.Animals.AddAsync(animal);
        await _database.SaveChangesAsync();

        return _mapper.Map<AnimalModel>(animal);
    }

    public async Task<AnimalModel> Update(long animalId, AnimalUpdateModel updateModel)
    {
        var animal = await FindAnimal(animalId);

        if (animal.ChipperId != updateModel.ChipperId)
        {
            var accountExists = await _database.Accounts.AnyAsync(x => x.Id == updateModel.ChipperId);
            if (accountExists == default)
                throw new NotFoundException("Account not found");
        }

        if (animal.ChippingLocationId != updateModel.ChippingLocationId)
        {
            var locationExists = await _database.LocationPoints.AnyAsync(x => x.Id == updateModel.ChippingLocationId);
            if (locationExists == default)
                throw new NotFoundException("Location not found");

            var firstLocation = await _database.AnimalVisitedLocations
                .OrderBy(x => x.DateTimeOfVisitLocationPoint)
                .FirstOrDefaultAsync(x => x.AnimalId == animalId);
            if (firstLocation?.LocationPointId == updateModel.ChippingLocationId)
                throw new InvalidOperationException();
        }

        animal = _mapper.Map(updateModel, animal);

        if (animal.LifeStatus == Animal.AnimalLifeStatus.DEAD && !animal.DeathDateTime.HasValue)
            animal.DeathDateTime = DateTime.UtcNow;

        _database.Animals.Update(animal);
        await _database.SaveChangesAsync();

        return await _database.Animals
            .ProjectTo<AnimalModel>(_mapper.ConfigurationProvider)
            .FirstAsync(x => x.Id == animalId);
    }

    public async Task Delete(long animalId)
    {
        var animal = await FindAnimal(animalId);

        var hasVisitedLocation = await _database.AnimalVisitedLocations.AnyAsync(x => x.AnimalId == animalId);
        if (hasVisitedLocation)
            throw new InvalidOperationException();

        _database.Animals.Remove(animal);
        await _database.SaveChangesAsync();
    }

    #region AnimalType

    public async Task<AnimalModel> AddType(long animalId, long typeId)
    {
        var animal = await FindFullAnimal(animalId);

        var animalType = await _database.AnimalTypes.FindAsync(typeId);
        if (animalType == default)
            throw new NotFoundException("AnimalType not found");

        if (animal.AnimalTypes.Contains(animalType))
            throw new ConflictException("Animal already contains type");

        animal.AnimalTypes.Add(animalType);
        _database.Animals.Update(animal);
        await _database.SaveChangesAsync();

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

        var newAnimalType = await _database.AnimalTypes.FindAsync(updateTypeModel.NewTypeId);
        if (newAnimalType == default)
            throw new NotFoundException("New Type not found");

        animal.AnimalTypes.Add(newAnimalType);
        _database.Animals.Update(animal);
        await _database.SaveChangesAsync();

        return _mapper.Map<AnimalModel>(animal);
    }

    public async Task<AnimalModel> DeleteType(long animalId, long typeId)
    {
        var animal = await FindFullAnimal(animalId);

        if (animal.AnimalTypes.Count == 1)
            throw new InvalidOperationException();

        var removedTypes = animal.AnimalTypes.RemoveAll(x => x.Id == typeId);
        if (removedTypes == 0)
            throw new NotFoundException("AnimalType not found");

        _database.Animals.Update(animal);
        await _database.SaveChangesAsync();

        return _mapper.Map<AnimalModel>(animal);
    }

    #endregion

    #region AnimalLocation

    public async Task<List<AnimalVisitedLocationModel>> SearchLocation(
        long animalId,
        AnimalSearchLocationModel searchLocationModel
    )
    {
        var query = _database.AnimalVisitedLocations.Where(x => x.AnimalId == animalId);

        if (searchLocationModel.StartDateTime != default)
            query = query.Where(x => x.DateTimeOfVisitLocationPoint < searchLocationModel.StartDateTime);

        if (searchLocationModel.EndDateTime != default)
            query = query.Where(x => x.DateTimeOfVisitLocationPoint > searchLocationModel.EndDateTime);

        return await query
            .OrderBy(x => x.Id)
            .Skip(searchLocationModel.From)
            .Take(searchLocationModel.Size)
            .ProjectTo<AnimalVisitedLocationModel>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<AnimalVisitedLocationModel> AddLocation(long animalId, long pointId)
    {
        var animal = await _database.Animals
            .Include(x => x.VisitedLocations)
            .FirstOrDefaultAsync(x => x.Id == animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");

        if (animal.LifeStatus == Animal.AnimalLifeStatus.DEAD)
            throw new InvalidOperationException();

        var location = await _database.LocationPoints.FindAsync(pointId);
        if (location == default)
            throw new NotFoundException("Location not found");

        if (animal.VisitedLocations.Count > 0)
        {
            var lastVisitedLocation = animal.VisitedLocations
                .OrderByDescending(x => x.DateTimeOfVisitLocationPoint)
                .First();
            if (lastVisitedLocation.LocationPointId == pointId)
                throw new InvalidOperationException("Point already exists");
        }
        else
        {
            if (animal.ChippingLocationId == pointId)
                throw new InvalidOperationException("ChippingLocationId equals new point id");
        }

        var animalVisitedLocation = new AnimalVisitedLocation()
        {
            Animal = animal,
            LocationPoint = location
        };

        await _database.AnimalVisitedLocations.AddAsync(animalVisitedLocation);
        await _database.SaveChangesAsync();

        return _mapper.Map<AnimalVisitedLocationModel>(animalVisitedLocation);
    }

    public async Task<AnimalVisitedLocationModel> UpdateLocation(
        long animalId,
        AnimalUpdateLocationModel updateLocationModel
    )
    {
        var animal = await _database.Animals
            .Include(x => x.VisitedLocations.OrderBy(l => l.DateTimeOfVisitLocationPoint))
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");
        
        // check new location point exists
        var location = await _database.LocationPoints.FindAsync(updateLocationModel.LocationPointId);
        if (location == default)
            throw new NotFoundException("Location not found");

        // get visitedLocation for update  
        var visitedLocation = animal.VisitedLocations
            .FirstOrDefault(x => x.Id == updateLocationModel.VisitedLocationPointId);
        if (visitedLocation == default)
            throw new NotFoundException("Visited location not found");

        if (visitedLocation.LocationPointId == updateLocationModel.LocationPointId)
            throw new InvalidOperationException("Location already exists");

        // check last visited point
        var lastVisitedLocation = animal.VisitedLocations
            .LastOrDefault(x => x.DateTimeOfVisitLocationPoint < visitedLocation.DateTimeOfVisitLocationPoint);
        if (lastVisitedLocation?.LocationPointId == updateLocationModel.LocationPointId)
            throw new InvalidOperationException();

        // check next visited point
        var nextVisitedLocation = animal.VisitedLocations
            .FirstOrDefault(x => x.DateTimeOfVisitLocationPoint > visitedLocation.DateTimeOfVisitLocationPoint);
        if (nextVisitedLocation?.LocationPointId == updateLocationModel.LocationPointId)
            throw new InvalidOperationException();

        // check ChippingLocationId not equals new location id 
        if (lastVisitedLocation == default && animal.ChippingLocationId == updateLocationModel.LocationPointId)
            throw new InvalidOperationException();

        visitedLocation.LocationPointId = updateLocationModel.LocationPointId;
        _database.AnimalVisitedLocations.Update(visitedLocation);
        await _database.SaveChangesAsync();

        return _mapper.Map<AnimalVisitedLocationModel>(visitedLocation);
    }

    public async Task DeleteLocation(long animalId, long visitedLocationPointId)
    {
        var animal = await _database.Animals
            .Include(x => x.VisitedLocations.OrderBy(l => l.DateTimeOfVisitLocationPoint))
            .FirstOrDefaultAsync(x => x.Id == animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");

        var visitedLocation = animal.VisitedLocations.FirstOrDefault(x => x.Id == visitedLocationPointId);
        if (visitedLocation == default)
            throw new NotFoundException("Visited location not found");

        // check locations are not repeated
        var lastVisitedLocation = animal.VisitedLocations
            .LastOrDefault(x => x.DateTimeOfVisitLocationPoint < visitedLocation.DateTimeOfVisitLocationPoint);
        var nextVisitedLocation = animal.VisitedLocations
            .FirstOrDefault(x => x.DateTimeOfVisitLocationPoint > visitedLocation.DateTimeOfVisitLocationPoint);
        if ((lastVisitedLocation != default || nextVisitedLocation != default) &&
            lastVisitedLocation?.LocationPointId == nextVisitedLocation?.LocationPointId)
            throw new InvalidOperationException();

        // check ChippingLocationId not equals first location id
        if (nextVisitedLocation?.LocationPointId == animal.ChippingLocationId)
            _database.AnimalVisitedLocations.Remove(nextVisitedLocation);

        _database.AnimalVisitedLocations.Remove(visitedLocation);
        await _database.SaveChangesAsync();
    }

    #endregion

    private async Task<Animal> FindFullAnimal(long animalId)
    {
        var animal = await _database.Animals
            .Include(x => x.AnimalTypes)
            .Include(x => x.VisitedLocations)
            .FirstOrDefaultAsync(x => x.Id == animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");

        return animal;
    }

    private async Task<Animal> FindAnimal(long animalId)
    {
        var animal = await _database.Animals.FindAsync(animalId);
        if (animal == default)
            throw new NotFoundException("Animal not found");

        return animal;
    }
}