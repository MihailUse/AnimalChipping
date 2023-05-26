using System.Reflection;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Persistence;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AnimalType> AnimalTypes => Set<AnimalType>();
    public DbSet<LocationPoint> LocationPoints => Set<LocationPoint>();
    public DbSet<AnimalVisitedLocation> AnimalVisitedLocations => Set<AnimalVisitedLocation>();
    public DbSet<Area> Areas => Set<Area>();

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<AnimalGender>();
        modelBuilder.HasPostgresEnum<AnimalLifeStatus>();
        modelBuilder.HasPostgresEnum<AccountRole>();
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}