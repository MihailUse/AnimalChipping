using NetTopologySuite.Geometries;

namespace Application.Entities;

public class Area
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public LineString AreaPoints { get; set; } = null!;
}
