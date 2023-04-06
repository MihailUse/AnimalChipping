using Application.Models.Area;

namespace Application.Interfaces;

public interface IAreaService
{
    Task<AreaModel> Get(long areaId);
    Task<AreaModel> Create(AreaCreateModel createModel);
    Task<AreaModel> Update(long areaId, AreaUpdateModel updateModel);
    Task Delete(long areaId);
}