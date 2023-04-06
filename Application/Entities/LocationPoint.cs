namespace Application.Entities;

public class LocationPoint
{
    public long Id { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public List<Animal> ChippedAnimals { get; set; } = null!;
    public List<AnimalVisitedLocation> AnimalVisitedLocations { get; set; } = null!;
}