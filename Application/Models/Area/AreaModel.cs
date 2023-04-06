namespace Application.Models.Area;

public class AreaModel
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public List<PointModel> AreaPoints { get; set; } = null!;   
}