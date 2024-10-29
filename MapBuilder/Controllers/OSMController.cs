using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Google.Common.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace MapBuilder.Controllers;

[ApiController]
[Route("{controller}")]
public class OSMController:ControllerBase
{
    ///osm/getdata/40.95876296470057/-74.28306490222923/40.44841428528941/-72.78636592806494
    [HttpGet("{action}/{latitude1}/{longitude1}/{latitude2}/{longitude2}")]
    public async Task<JsonObject?> GetData(double latitudelo, double longitudelo, double latitudehi, double longitudehi)
    {
        string apiUrl = "https://overpass-api.de/api/interpreter";
        string query = $"[out:json][maxsize:1073741824][timeout:900];way({latitudelo},{longitudelo},{latitudehi},{longitudehi});out geom;";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain")).Result;
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonObject>(jsonString);
        }
    }
    [HttpGet("{action}/{cellToken}")]
    public async Task<JsonObject?> GetData(string cellToken)
    {
        S2Cell cell = new S2Cell(S2CellId.FromToken(cellToken));
        
        string apiUrl = "https://overpass-api.de/api/interpreter";
        string query = $"[out:json][maxsize:1073741824][timeout:900];way({cell.RectBound.LatLo},{cell.RectBound.LngLo},{cell.RectBound.LatHi},{cell.RectBound.LngHi});out geom;";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain")).Result;
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonObject>(jsonString);
        }
    }
}