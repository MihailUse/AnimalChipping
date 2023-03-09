using Application.Models.Animal;
using Application.Models.AnimalVisitedLocation;

namespace Application.Interfaces;

public interface IAnimalService
{
    Task<List<AnimalModel>> Search(AnimalSearchModel searchModel);
    Task<AnimalModel> Get(long animalId);
    Task<AnimalModel> Create(AnimalCreateModel createModel);
    Task<AnimalModel> Update(long animalId, AnimalUpdateModel updateModel);
    Task Delete(long animalId);

    Task<AnimalModel> AddType(long animalId, long typeId);
    Task<AnimalModel> UpdateType(long animalId, AnimalUpdateTypeModel updateTypeModel);
    Task<AnimalModel> DeleteType(long animalId, long typeId);

    Task<List<AnimalVisitedLocationModel>> SearchLocation(long animalId, AnimalSearchLocationModel searchLocationModel);
    Task<AnimalVisitedLocationModel> AddLocation(long animalId, long pointId);
    Task<AnimalVisitedLocationModel> UpdateLocation(long animalId, AnimalUpdateLocationModel updateLocationModel);
    Task DeleteLocation(long animalId, long visitedLocationPointId);
}