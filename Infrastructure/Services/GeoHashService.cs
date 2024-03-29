using Application.Interfaces;
using GeoHash.NetCore.Utilities.Encoders;

namespace Infrastructure.Services;

public class GeoHashService : IGeoHashService
{
    public string GetOpenLocationCode(double latitude, double longitude)
    {
        var encoder = new GeoHashEncoder<string>();
        return encoder.Encode(latitude, longitude);
    }
}