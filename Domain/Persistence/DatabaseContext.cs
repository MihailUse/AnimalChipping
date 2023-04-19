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
        
        modelBuilder.Entity<Account>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<Account>().HasData(
            new Account()
            {
                Id = 1,
                FirstName = "adminFirstName",
                LastName = "adminLastName",
                Email = "admin@simbirsoft.com",
                Password = "qwerty123",
                Role = AccountRole.ADMIN
            },
            new Account()
            {
                Id = 2,
                FirstName = "chipperFirstName",
                LastName = "chipperLastName",
                Email = "chipper@simbirsoft.com",
                Password = "qwerty123",
                Role = AccountRole.CHIPPER
            },
            new Account()
            {
                Id = 3,
                FirstName = "userFirstName",
                LastName = "userLastName",
                Email = "user@simbirsoft.com",
                Password = "qwerty123",
                Role = AccountRole.USER
            });

        base.OnModelCreating(modelBuilder);
    }
}