namespace Application.Entities;

public class AnimalVisitedLocation
{
    public long Id { get; set; }
    public long AnimalId { get; set; }
    public long LocationPointId { get; set; }
    public DateTime DateTimeOfVisitLocationPoint { get; set; } = DateTime.UtcNow;

    public Animal Animal { get; set; } = null!;
    public LocationPoint LocationPoint { get; set; } = null!;
}