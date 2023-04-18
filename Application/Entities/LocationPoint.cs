using NetTopologySuite.Geometries;

namespace Application.Entities;

public class LocationPoint
{
    public long Id { get; set; }
    public Point Point { get; set; } = null!;

    public List<Animal> ChippedAnimals { get; set; } = null!;
    public List<AnimalVisitedLocation> AnimalVisitedLocations { get; set; } = null!;
}