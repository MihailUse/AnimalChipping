using Application.Models.AnimalType;

namespace Application.Interfaces;

public interface IAnimalTypeService
{
    Task<AnimalTypeModel> Get(long typeId);
    Task<AnimalTypeModel> Create(AnimalTypeCreateModel createModel);
    Task<AnimalTypeModel> Update(long typeId, AnimalTypeUpdateModel updateModel);
    Task Delete(long typeId);
}