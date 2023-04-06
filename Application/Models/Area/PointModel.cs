namespace Application.Models.Area;

public class PointModel
{
    public double Longitude { get; }
    public double Latitude { get; }

    public PointModel(double longitude, double latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }
}