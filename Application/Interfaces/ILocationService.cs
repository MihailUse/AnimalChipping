using Application.Models.Area;
using Application.Models.Location;

namespace Application.Interfaces;

public interface ILocationService
{
    Task<LocationPointModel> Get(int locationId);
    Task<LocationPointModel> Create(LocationPointCreateModel createModel);
    Task<LocationPointModel> Update(long pointId, LocationPointUpdateModel updateModel);
    Task Delete(long pointId);
    Task<string> GetIdByPoint(PointModel model);
    Task<string> GetPlusCodeHash(PointModel model);
    Task<string> GetPlusCodeBase64(PointModel model);
    Task<string> GetHashV3(PointModel model);
}