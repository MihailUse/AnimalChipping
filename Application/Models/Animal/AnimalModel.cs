namespace Application.Models.Animal;

public class AnimalModel
{
    public long Id { get; set; }
    public float Weight { get; set; }
    public float Length { get; set; }
    public float Height { get; set; }
    public string Gender { get; set; } = null!;
    public string LifeStatus { get; set; } = null!;
    public DateTimeOffset ChippingDateTime { get; set; }
    public DateTimeOffset? DeathDateTime { get; set; }

    public int ChipperId { get; set; }
    public long ChippingLocationId { get; set; }

    public List<long> AnimalTypes { get; set; } = null!;
    public List<long> VisitedLocations { get; set; } = null!;
}