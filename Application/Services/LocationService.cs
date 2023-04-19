using System.Security.Cryptography;
using System.Text;
using Application.Entities;
using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Area;
using Application.Models.Location;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Application.Services;

internal class LocationService : ILocationService
{
    private readonly IMapper _mapper;
    private readonly IDatabaseContext _database;
    private readonly IGeoHashService _hashService;

    public LocationService(IMapper mapper, IDatabaseContext database, IGeoHashService hashService)
    {
        _mapper = mapper;
        _database = database;
        _hashService = hashService;
    }

    public async Task<LocationPointModel> Get(int locationId)
    {
        var location = await _database.LocationPoints
            .ProjectTo<LocationPointModel>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == locationId);

        if (location == default)
            throw new NotFoundException("Location not found");

        return location;
    }

    public async Task<LocationPointModel> Create(LocationPointCreateModel createModel)
    {
        var isExists = await _database.LocationPoints
            .AnyAsync(x => x.Point.X == createModel.Longitude && x.Point.Y == createModel.Latitude);
        if (isExists)
            throw new ConflictException("Location already exists");

        var locationPoint = _mapper.Map<LocationPoint>(createModel);
        await _database.LocationPoints.AddAsync(locationPoint);
        await _database.SaveChangesAsync();

        return _mapper.Map<LocationPointModel>(locationPoint);
    }

    public async Task<LocationPointModel> Update(long pointId, LocationPointUpdateModel updateModel)
    {
        var location = await _database.LocationPoints.FindAsync(pointId);
        if (location == default)
            throw new NotFoundException("Location not found");

        var isExists = await _database.LocationPoints.AnyAsync(x =>
            x.Id != pointId &&
            x.Point.X == updateModel.Longitude &&
            x.Point.Y == updateModel.Latitude);
        if (isExists)
            throw new ConflictException("Location already exists");

        location = _mapper.Map(updateModel, location);
        _database.LocationPoints.Update(location);
        await _database.SaveChangesAsync();

        return _mapper.Map<LocationPointModel>(location);
    }

    public async Task Delete(long pointId)
    {
        var location = await _database.LocationPoints.FindAsync(pointId);
        if (location == default)
            throw new NotFoundException("Location not found");

        var hasAnimals = await _database.Animals.AnyAsync(x =>
            x.ChippingLocationId == pointId ||
            x.VisitedLocations.Any(l => l.LocationPointId == pointId));
        if (hasAnimals)
            throw new InvalidOperationException();

        _database.LocationPoints.Remove(location);
        await _database.SaveChangesAsync();
    }

    public async Task<string> GetIdByPoint(PointModel model)
    {
        var location = await FindLocation(model);
        return location.Id.ToString();
    }


    public async Task<string> GetPlusCodeHash(PointModel model)
    {
        var location = await FindLocation(model);
        return _hashService.GetPlusCodeHash(location.Point.Y, location.Point.X);
    }

    public async Task<string> GetPlusCodeBase64(PointModel model)
    {
        var location = await FindLocation(model);
        var plusCode = _hashService.GetPlusCodeHash(location.Point.Y, location.Point.X);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plusCode));
    }

    public async Task<string> GetHashV3(PointModel model)
    {
        var location = await FindLocation(model);
        var plusCode = _hashService.GetPlusCodeHash(location.Point.Y, location.Point.X);

        using var md5 = MD5.Create();
        var md5Encoded = md5.ComputeHash(Encoding.ASCII.GetBytes(plusCode));
        return Convert.ToBase64String(md5Encoded.Reverse().ToArray());
    }

    private async Task<LocationPoint> FindLocation(PointModel model)
    {
        var point = new Point(model.Longitude, model.Latitude);
        var location = await _database.LocationPoints.FirstOrDefaultAsync(x => x.Point == point);
        if (location == default)
            throw new NotFoundException("Location not found");

        return location;
    }
}