namespace Application.Models.AnimalVisitedLocation;

public class AnimalVisitedLocationModel
{
    public long Id { get; set; }
    public long LocationPointId { get; set; }
    public DateTimeOffset DateTimeOfVisitLocationPoint { get; set; }
}