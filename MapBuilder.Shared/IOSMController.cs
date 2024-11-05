using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

namespace MapBuilder.Shared;

public interface IOSMController
{
    public Task<JToken?> GetDataFromBox(double latitudeLo, double longitudeLo, double latitudeHi,
        double longitudeHi);
    public Task<JsonObject?> GetData(string cellToken);
}