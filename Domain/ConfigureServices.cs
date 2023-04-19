using Domain.Interfaces;
using Domain.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class ConfigureServices
{
    public static IServiceCollection AddDomainServices(
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

        return services;
    }
}