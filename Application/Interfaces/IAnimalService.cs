using Application.Models.Animal;

namespace Application.Interfaces;

public interface IAnimalService
{
    Task<AnimalModel> Get(long animalId);
    Task<AnimalModel> Create(AnimalCreateModel createModel);
    Task<AnimalModel> Update(long animalId, AnimalUpdateModel updateModel);
    Task Delete(long animalId);
    Task<List<AnimalModel>> Search(AnimalSearchModel searchModel);
}