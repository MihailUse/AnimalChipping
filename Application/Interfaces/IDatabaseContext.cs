using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IDatabaseContext
{
    public DbSet<Account> Accounts { get; }
    public DbSet<Animal> Animals { get; }
    public DbSet<AnimalType> AnimalTypes { get; }
    public DbSet<LocationPoint> LocationPoints { get; }
    public DbSet<AnimalVisitedLocation> AnimalVisitedLocations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());
}