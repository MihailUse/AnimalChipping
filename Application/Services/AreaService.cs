using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Area;
using AutoMapper;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Area = Domain.Entities.Area;

namespace Application.Services;

public class AreaService : IAreaService
{
    private readonly IMapper _mapper;
    private readonly IDatabaseContext _database;

    public AreaService(IMapper mapper, IDatabaseContext database)
    {
        _mapper = mapper;
        _database = database;
    }

    public async Task<AreaModel> Get(long areaId)
    {
        var area = await _database.Areas.FindAsync(areaId);
        if (area == default)
            throw new NotFoundException("Area not found");

        return _mapper.Map<AreaModel>(area);
    }

    public async Task<AreaModel> Create(AreaCreateModel createModel)
    {
        var area = _mapper.Map<Area>(createModel);

        var lineStringWithOffsetPoints = area.AreaPoints.Coordinates.Skip(1).Append(area.AreaPoints.Coordinates.First());
        var lineStringWithOffset = new LineString(lineStringWithOffsetPoints.ToArray());
        if (!area.AreaPoints.IsSimple || !area.AreaPoints.IsValid || !lineStringWithOffset.IsSimple)
            throw new InvalidOperationException("Invalid polygon points");

        var hasExistsArea = await _database.Areas.AnyAsync(x => x.Name == area.Name);
        if (hasExistsArea)
            throw new ConflictException("Name or area points already exists");

        var isInvalid = await _database.Areas.AnyAsync(x => x.AreaPoints.Crosses(area.AreaPoints));
        if (isInvalid)
            throw new InvalidOperationException();

        await _database.Areas.AddAsync(area);
        await _database.SaveChangesAsync();

        return _mapper.Map<AreaModel>(area);
    }

    public async Task<AreaModel> Update(long areaId, AreaUpdateModel updateModel)
    {
        var area = await _database.Areas.FindAsync(areaId);
        if (area == default)
            throw new NotFoundException("Area not found");

        area = _mapper.Map(updateModel, area);
        var lineStringWithOffsetPoints = area.AreaPoints.Coordinates.Skip(1).Append(area.AreaPoints.Coordinates.First());
        var lineStringWithOffset = new LineString(lineStringWithOffsetPoints.ToArray());
        if (!area.AreaPoints.IsSimple || !area.AreaPoints.IsValid || !lineStringWithOffset.IsSimple)
            throw new InvalidOperationException("Invalid polygon points");

        var hasExistsArea = await _database.Areas.AnyAsync(x => x.Id != areaId && x.Name == area.Name);
        if (hasExistsArea)
            throw new ConflictException("Name or area points already exists");

        _database.Areas.Update(area);
        await _database.SaveChangesAsync();

        return _mapper.Map<AreaModel>(area);
    }

    public async Task Delete(long areaId)
    {
        var area = await _database.Areas.FindAsync(areaId);
        if (area == default)
            throw new NotFoundException("Area not found");

        _database.Areas.Remove(area);
        await _database.SaveChangesAsync();
    }

    public async Task<AnalyticModel> GetAnalytic(long areaId, GetAnalyticModel model)
    {
        var area = await _database.Areas.FindAsync(areaId);
        if (area == default)
            throw new NotFoundException("Area not found");

        var areaPolygon = new Polygon(new LinearRing(area.AreaPoints.Coordinates.Append(area.AreaPoints.Coordinates.First()).ToArray()));
        var animals = await _database.Animals
            .Include(x => x.VisitedLocations
                .Where(l =>
                    l.DateTimeOfVisitLocationPoint >= model.StartDate &&
                    l.DateTimeOfVisitLocationPoint <= model.EndDate
                )
                .OrderBy(l => l.DateTimeOfVisitLocationPoint))
            .ThenInclude(x => x.LocationPoint)
            .Include(x => x.AnimalTypes)
            .Include(x => x.ChippingLocation)
            .Where(x =>
                (
                    x.ChippingDateTime >= model.StartDate &&
                    x.ChippingDateTime <= model.EndDate &&
                    (areaPolygon.Contains(x.ChippingLocation.Point) || areaPolygon.Touches(x.ChippingLocation.Point))
                ) ||
                x.VisitedLocations.Any(l =>
                    l.DateTimeOfVisitLocationPoint >= model.StartDate &&
                    l.DateTimeOfVisitLocationPoint <= model.EndDate &&
                    (areaPolygon.Contains(l.LocationPoint.Point) || areaPolygon.Touches(l.LocationPoint.Point))
                )
            )
            .ToListAsync();

        var analyticModel = new AnalyticModel();
        var typeStatistics = animals.SelectMany(x => x.AnimalTypes)
            .DistinctBy(x => x.Id)
            .ToDictionary(x => x.Id, x => new AnimalsAnalyticModel() { AnimalTypeId = x.Id, AnimalType = x.Type });

        foreach (var animal in animals)
        {
            var visitedPoints = animal.VisitedLocations.Select(x => x.LocationPoint.Point).ToList();
            if (animal.ChippingDateTime >= model.StartDate && animal.ChippingDateTime <= model.EndDate)
                visitedPoints.Insert(0, animal.ChippingLocation.Point);

            var currentLocationPoint = visitedPoints.Last();
            var inArea = areaPolygon.Contains(currentLocationPoint) || areaPolygon.Touches(currentLocationPoint);
            var isArrived = false;
            var isGone = false;

            for (var i = 0; i < visitedPoints.Count - 1; i++)
            {
                var firstPoint = visitedPoints[i];
                var secondPoint = visitedPoints[i + 1];
                var containsFirstPoint = areaPolygon.Contains(firstPoint) || areaPolygon.Touches(firstPoint);
                var containsSecondPoint = areaPolygon.Contains(secondPoint) || areaPolygon.Touches(secondPoint);

                if (containsFirstPoint && !containsSecondPoint)
                    isGone = true;
                else if (!containsFirstPoint && containsSecondPoint)
                    isArrived = true;
            }

            if (isArrived)
                analyticModel.TotalAnimalsArrived++;

            if (isGone)
                analyticModel.TotalAnimalsGone++;

            if (inArea)
                analyticModel.TotalQuantityAnimals++;

            // animalsAnalytics
            foreach (var typeStatistic in animal.AnimalTypes.Select(type => typeStatistics[type.Id]))
            {
                if (isArrived)
                    typeStatistic.AnimalsArrived++;

                if (isGone)
                    typeStatistic.AnimalsGone++;

                if (inArea)
                    typeStatistic.QuantityAnimals++;
            }
        }

        analyticModel.AnimalsAnalytics = typeStatistics.Values.ToList();
        return analyticModel;
    }
}