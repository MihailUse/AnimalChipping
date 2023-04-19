namespace Application.Interfaces;

public interface IGeoHashService
{
    public string GetPlusCodeHash(double latitude, double longitude);
}