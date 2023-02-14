namespace Domain.Entities;

public class AnimalVisitedLocation
{
    public long AnimalId { get; set; }
    public long LocationPointId { get; set; }
    public DateTime DateTimeOfVisitLocationPoint { get; set; } = DateTime.UtcNow;

    public virtual Animal Animal { get; set; } = null!;
    public virtual LocationPoint LocationPoint { get; set; } = null!;
}