using System.Reflection;
using Application.Interfaces;
using Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IAnimalService, AnimalService>();
        services.AddScoped<IAnimalTypeService, AnimalTypeService>();

        return services;
    }
}