namespace Domain.Entities;

public class LocationPoint
{
    public long Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public virtual List<Animal> ChippedAnimals { get; set; } = null!;
    public virtual List<AnimalVisitedLocation> AnimalVisitedLocations { get; set; } = null!;
}