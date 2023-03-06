using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Location;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

internal class LocationService : ILocationService
{
    private readonly IMapper _mapper;
    private readonly IDatabaseContext _context;

    public LocationService(IMapper mapper, IDatabaseContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<LocationPointModel> Get(int locationId)
    {
        var location = await _context.LocationPoints
            .ProjectTo<LocationPointModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == locationId);

        if (location == default)
            throw new NotFoundException("Location not found");

        return location;
    }

    public async Task<LocationPointModel> Create(LocationPointCreateModel createModel)
    {
        var isExists = await _context.LocationPoints
            .AnyAsync(x => x.Longitude == createModel.Longitude && x.Latitude == createModel.Latitude);
        if (isExists)
            throw new ConflictException("Location already exists");

        var locationPoint = _mapper.Map<LocationPoint>(createModel);
        await _context.LocationPoints.AddAsync(locationPoint);
        await _context.SaveChangesAsync();

        return _mapper.Map<LocationPointModel>(locationPoint);
    }

    public async Task<LocationPointModel> Update(long pointId, LocationPointUpdateModel updateModel)
    {
        var location = await _context.LocationPoints.FindAsync(pointId);
        if (location == default)
            throw new NotFoundException("Location not found");

        var isExists = await _context.LocationPoints.AnyAsync(x =>
            x.Id != pointId &&
            x.Latitude == updateModel.Latitude &&
            x.Longitude == updateModel.Longitude);
        if (isExists)
            throw new ConflictException("Location already exists");

        location = _mapper.Map(updateModel, location);
        location.Id = pointId;

        _context.LocationPoints.Update(location);
        await _context.SaveChangesAsync();

        return _mapper.Map<LocationPointModel>(location);
    }

    public async Task Delete(long pointId)
    {
        var location = await _context.LocationPoints.FindAsync(pointId);
        if (location == default)
            throw new NotFoundException("Location not found");

        var hasAnimals = await _context.Animals.AnyAsync(x =>
            x.ChippingLocationId == pointId ||
            x.VisitedLocations.Select(v => v.LocationPointId).Contains(pointId));
        if (hasAnimals)
            throw new InvalidOperationException();

        _context.LocationPoints.Remove(location);
        await _context.SaveChangesAsync();
    }
}