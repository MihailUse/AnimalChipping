using Application.Models.Account;
using Application.Models.Animal;
using Application.Models.AnimalType;
using Application.Models.Location;
using AutoMapper;
using Domain.Entities;

namespace Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // account
        CreateMap<Account, AccountModel>().ReverseMap();
        CreateMap<AccountUpdateModel, Account>();
        CreateMap<AccountCreateModel, Account>();

        // location
        CreateMap<LocationPoint, LocationPointModel>().ReverseMap();
        CreateMap<LocationPointCreateModel, LocationPoint>();
        CreateMap<LocationPointUpdateModel, LocationPoint>();

        // animal
        CreateProjection<Animal, AnimalModel>()
            .ForMember(d => d.Gender, m => m.MapFrom(s => s.Gender.ToString()))
            .ForMember(d => d.LifeStatus, m => m.MapFrom(s => s.LifeStatus.ToString()))
            .ForMember(d => d.AnimalTypes, m => m.MapFrom(s => s.AnimalTypes.Select(x => x.Id)))
            .ForMember(d => d.VisitedLocations, m => m.MapFrom(s => s.VisitedLocations.Select(x => x.LocationPointId)));
        CreateMap<AnimalCreateModel, Animal>()
            .ForMember(d => d.Gender, m => m.MapFrom(s => Enum.Parse<Animal.AnimalGender>(s.Gender)));

        // animal type
        CreateMap<AnimalTypeModel, AnimalType>().ReverseMap();
        CreateMap<AnimalTypeCreateModel, AnimalType>();
    }
}