namespace Application.Models.Area;

public class AreaCreateModel
{
    public string Name { get; set; } = null!;
    public List<PointModel> AreaPoints { get; set; } = null!;   
}