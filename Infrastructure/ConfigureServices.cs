﻿using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                o =>
                {
                    o.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName);
                    o.UseNetTopologySuite();
                }));

        services.AddSingleton<IGeoHashService, GeoHashService>();

        return services;
    }
}