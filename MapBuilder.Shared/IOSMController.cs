using System.Text.Json.Nodes;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json.Linq;

namespace MapBuilder.Shared;

public interface IOSMController
{
    public Task<OSM?> GetDataFromBox(double latitudeLo, double longitudeLo, double latitudeHi,
        double longitudeHi);
    public Task<OSM?> GetDataFromToken(string cellToken);
    public Task<string?> GetData(string output, int level, double latitude, double longitude);
}