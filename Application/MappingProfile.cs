using Application.Entities;
using Application.Models.Account;
using Application.Models.Animal;
using Application.Models.AnimalType;
using Application.Models.AnimalVisitedLocation;
using Application.Models.Area;
using Application.Models.Location;
using AutoMapper;
using NetTopologySuite.Geometries;

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
        CreateMap<LocationPointCreateModel, LocationPoint>()
            .ForMember(d => d.Point, m => m.MapFrom(s => new Point(s.Longitude, s.Latitude)));
        CreateMap<LocationPointUpdateModel, LocationPoint>()
            .ForMember(d => d.Point, m => m.MapFrom(s => new Point(s.Longitude, s.Latitude)));
        CreateMap<LocationPoint, LocationPointModel>()
            .ForMember(d => d.Longitude, m => m.MapFrom(s => s.Point.X))
            .ForMember(d => d.Latitude, m => m.MapFrom(s => s.Point.Y));
        CreateMap<LocationPointModel, LocationPoint>()
            .ForMember(d => d.Point, m => m.MapFrom(s => new Point(s.Longitude, s.Latitude)));
        
        // AnimalVisitedLocation 
        CreateMap<AnimalVisitedLocation, AnimalVisitedLocationModel>();

        // Area
        CreateMap<List<PointModel>, LineString>()
            .ConvertUsing((source, destination, context) =>
            {
                return new LineString(source
                    .Select(p => new Coordinate(p.Longitude, p.Latitude))
                    .ToArray()
                );
            });

        CreateMap<LineString, List<PointModel>>()
            .ConvertUsing((source, destination, context) =>
            {
                return source.Coordinates
                    .Select(x => new PointModel(x.X, x.Y))
                    .ToList();
            });

        CreateMap<AreaCreateModel, Area>();
        CreateMap<AreaUpdateModel, Area>();
        CreateMap<Area, AreaModel>().ReverseMap();

        #endregion
    }
}