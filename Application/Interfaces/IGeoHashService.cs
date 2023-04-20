namespace Application.Interfaces;

public interface IGeoHashService
{
    public string GetOpenLocationCode(double latitude, double longitude);
}