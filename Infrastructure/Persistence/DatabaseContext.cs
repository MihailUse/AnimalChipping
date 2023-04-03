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