namespace Domain.Entities;

public class AnimalType
{
    public long Id { get; set; }
    public string Type { get; set; } = null!;

    public List<Animal> Animals { get; set; } = null!;
}