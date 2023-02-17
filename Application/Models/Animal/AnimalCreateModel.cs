namespace Application.Models.Animal;

public class AnimalCreateModel
{
    public float Weight { get; set; }
    public float Length { get; set; }
    public float Height { get; set; }
    public string Gender { get; set; } = null!;

    public int ChipperId { get; set; }
    public long ChippingLocationId { get; set; }

    public List<long> AnimalTypes { get; set; } = null!;
}