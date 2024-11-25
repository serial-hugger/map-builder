using System.Text.Json.Nodes;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json.Linq;

namespace MapBuilder.Shared;

public interface IOSMController
{
    public Task<OSM?> GetDataFromBox(double latitudeLo, double longitudeLo, double latitudeHi,
        double longitudeHi, CancellationToken ct);
    public Task<OSM?> GetData(string cellToken, CancellationToken ct);
}