namespace Application.Models.Area;

public class PointModel
{
    public double Longitude { get; set; }
    public double Latitude { get; set; }

    public PointModel()
    {
    }

    public PointModel(double longitude, double latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }
}