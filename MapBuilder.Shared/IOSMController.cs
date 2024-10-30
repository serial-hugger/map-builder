using System.Text.Json.Nodes;

namespace MapBuilder.Shared;

public interface IOSMController
{
    public Task<JsonObject?> GetDataFromBox(double latitudeLo, double longitudeLo, double latitudeHi,
        double longitudeHi);
    public Task<JsonObject?> GetData(string cellToken);
}