namespace Domain.Entities;

public class Animal
{
    public long Id { get; set; }
    public float Weight { get; set; }
    public float Length { get; set; }
    public float Height { get; set; }
    public AnimalGender Gender { get; set; }
    public AnimalLifeStatus LifeStatus { get; set; } = AnimalLifeStatus.ALIVE;
    public DateTime ChippingDateTime { get; set; }
    public DateTime? DeathDateTime { get; set; }

    public int ChipperId { get; set; }
    public long ChippingLocationId { get; set; }

    public Account Chipper { get; set; } = null!;
    public LocationPoint ChippingLocation { get; set; } = null!;
    public List<AnimalType> AnimalTypes { get; set; } = null!;
    public List<AnimalVisitedLocation> VisitedLocations { get; set; } = null!;
}