using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Area;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Area = Application.Entities.Area;

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

        var lineStringWithOffset =
            new LineString(area.AreaPoints.Coordinates.Skip(1).Append(area.AreaPoints.Coordinates.First()).ToArray());
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
        var lineStringWithOffset =
            new LineString(area.AreaPoints.Coordinates.Skip(1).Append(area.AreaPoints.Coordinates.First()).ToArray());
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
}