namespace Application.Models.Area;

public class AnalyticModel
{
    public long TotalQuantityAnimals { get; set; }
    public long TotalAnimalsArrived { get; set; }
    public long TotalAnimalsGone { get; set; }
    public List<AnimalsAnalyticModel> AnimalsAnalytics { get; set; } = new();
}