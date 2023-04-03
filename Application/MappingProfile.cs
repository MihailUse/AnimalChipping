using Application.Models.Account;
using Application.Models.Animal;
using Application.Models.AnimalType;
using Application.Models.AnimalVisitedLocation;
using Application.Models.Location;
using AutoMapper;
using Domain.Entities;

namespace Application;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        #region Projections

        // animal
        CreateProjection<Animal, AnimalModel>()
            .ForMember(d => d.Gender, m => m.MapFrom(s => s.Gender.ToString()))
            .ForMember(d => d.LifeStatus, m => m.MapFrom(s => s.LifeStatus.ToString()))
            .ForMember(d => d.AnimalTypes, m => m.MapFrom(s => s.AnimalTypes.Select(x => x.Id)))
            .ForMember(d => d.VisitedLocations, m => m.MapFrom(s => s.VisitedLocations.Select(x => x.LocationPointId)));

        // AnimalVisitedLocation 
        CreateProjection<AnimalVisitedLocation, AnimalVisitedLocationModel>();

        #endregion

        #region Mappings

        // animal
        CreateMap<AnimalCreateModel, Animal>()
            .ForMember(d => d.AnimalTypes, m => m.Ignore())
            .ForMember(d => d.Gender, m => m.MapFrom(s => Enum.Parse<AnimalGender>(s.Gender)));
        CreateMap<AnimalUpdateModel, Animal>()
            .ForMember(d => d.Gender, m => m.MapFrom(s => Enum.Parse<AnimalGender>(s.Gender)))
            .ForMember(d => d.LifeStatus, m => m.MapFrom(s => Enum.Parse<AnimalLifeStatus>(s.LifeStatus)));
        CreateMap<Animal, AnimalModel>()
            .ForMember(d => d.Gender, m => m.MapFrom(s => s.Gender.ToString()))
            .ForMember(d => d.LifeStatus, m => m.MapFrom(s => s.LifeStatus.ToString()))
            .ForMember(d => d.AnimalTypes, m => m.MapFrom(s => s.AnimalTypes.Select(x => x.Id)))
            .ForMember(d => d.VisitedLocations, m => m.MapFrom(s => s.VisitedLocations.Select(x => x.Id)));

        // animal type
        CreateMap<AnimalTypeCreateModel, AnimalType>();
        CreateMap<AnimalTypeUpdateModel, AnimalType>();
        CreateMap<AnimalTypeModel, AnimalType>().ReverseMap();

        // account
        CreateMap<AccountRegistrationModel, Account>();
        CreateMap<AccountCreateModel, Account>()
            .ForMember(d => d.Role, m => m.MapFrom(s => Enum.Parse<AccountRole>(s.Role)));
        CreateMap<AccountUpdateModel, Account>();
        CreateMap<Account, AccountModel>().ReverseMap();

        // location
        CreateMap<LocationPointCreateModel, LocationPoint>();
        CreateMap<LocationPointUpdateModel, LocationPoint>();
        CreateMap<LocationPoint, LocationPointModel>().ReverseMap();

        // AnimalVisitedLocation 
        CreateMap<AnimalVisitedLocation, AnimalVisitedLocationModel>();

        #endregion
    }
}