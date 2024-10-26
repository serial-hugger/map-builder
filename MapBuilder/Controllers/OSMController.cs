using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

namespace MapBuilder.Controllers;

[ApiController]
[Route("{controller}")]
public class OSMController:ControllerBase
{
    ///osm/getdata/40.95876296470057/-74.28306490222923/40.44841428528941/-72.78636592806494
    [HttpGet("{action}/{latitude1}/{longitude1}/{latitude2}/{longitude2}")]
    public async Task<JsonObject> GetData(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        string apiUrl = "https://overpass-api.de/api/interpreter";
        string query = $"[out:json][maxsize:1073741824][timeout:900];way(51.15,7.12,51.16,7.13);out geom;";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain")).Result;
            var jsonString = response.Content.ReadAsStringAsync().Result;
            return JsonSerializer.Deserialize<JsonObject>(jsonString);
        }
    }
}