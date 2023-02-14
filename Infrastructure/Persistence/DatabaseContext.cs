using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class DatabaseContext : DbContext, IDatabaseContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<AnimalType> AnimalTypes => Set<AnimalType>();
    public DbSet<LocationPoint> LocationPoints => Set<LocationPoint>();
    public DbSet<AnimalVisitedLocation> AnimalVisitedLocations => Set<AnimalVisitedLocation>();

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnimalVisitedLocation>()
            .HasKey(x => new { x.AnimalId, x.LocationPointId });

        modelBuilder.Entity<Account>()
            .HasIndex(x => x.Email)
            .IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }
}