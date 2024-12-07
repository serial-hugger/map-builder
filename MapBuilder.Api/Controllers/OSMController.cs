using System.Text;
using System.Text.Json.Nodes;
using Google.Common.Geometry;
using Microsoft.AspNetCore.Mvc;
using MapBuilder.Data;
using MapBuilder.Shared;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace MapBuilder.Api.Controllers;

[ApiController]
[Route("{controller}")]
public class OSMController:ControllerBase, IOSMController
{
    private readonly CellContext _context;
    private readonly ICellRepository _cellRepository;
    private IOSMController _iosmControllerImplementation;

    public OSMController()
    {
        _context = new CellContext();
        _cellRepository = new CellRepository();
    }
    ///osm/getdata/40.95876296470057/-74.28306490222923/40.44841428528941/-72.78636592806494
    [HttpGet("{action}/{latitudeLo}/{longitudeLo}/{latitudeHi}/{longitudeHi}")]
    public async Task<OSM?> GetDataFromBox(double latitudeLo, double longitudeLo, double latitudeHi, double longitudeHi, CancellationToken ct)
    {
        string apiUrl = "https://overpass-api.de/api/interpreter";
        string query = $"[out:json][maxsize:1073741824][timeout:900];node({latitudeLo},{longitudeLo},{latitudeHi},{longitudeHi});<<;out geom;";
        Console.WriteLine($"Query: {query}");
        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage response= client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain"),ct).Result;

            var jsonString = await response.Content.ReadAsStringAsync(ct);
            return JsonConvert.DeserializeObject<OSM>(jsonString);
        }
    }

    [HttpGet("{action}/{cellToken}")]
    public async Task<OSM?> GetDataFromToken(string cellToken, CancellationToken ct)
    {
        S2Cell cell = new S2Cell(S2CellId.FromToken(cellToken));
        
        Cell cellModel = await _cellRepository.GetCellByTokenAsync(cellToken);

        if (cellModel == null)
        {
            string apiUrl = "https://overpass-api.de/api/interpreter";
            string query =
                $"[out:json][maxsize:1073741824][timeout:900];way({cell.RectBound.LatLo},{cell.RectBound.LngLo},{cell.RectBound.LatHi},{cell.RectBound.LngHi});out geom;";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var response = client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain"),ct).Result;
                var jsonString = await response.Content.ReadAsStringAsync(ct);
                return JsonSerializer.Deserialize<OSM>(jsonString);
            }
        }
        else
        {
            var jsonString = JsonSerializer.Serialize(cellModel);
            return JsonSerializer.Deserialize<OSM>(jsonString);
        }
    }
    [HttpGet("{action}/{level}/{latitude}/{longitude}")]
    public async Task<string?> GetData(int level, double latitude, double longitude, CancellationToken ct)
    {
        var coord = S2LatLng.FromDegrees(latitude, longitude);
        var token = S2CellId.FromLatLng(coord).ParentForLevel(level).ToToken();
        S2Cell cell = new S2Cell(S2CellId.FromToken(token));
        
            string apiUrl = "https://overpass-api.de/api/interpreter";
            string query =
                $"[out:json][maxsize:1073741824][timeout:900];way({cell.RectBound.LatLo.ToString().Replace("d","")},{cell.RectBound.LngLo.ToString().Replace("d","")},{cell.RectBound.LatHi.ToString().Replace("d","")},{cell.RectBound.LngHi.ToString().Replace("d","")});out geom;";

            using (var client = new HttpClient())
            {
                Console.WriteLine($"Query: {query}");
                //client.DefaultRequestHeaders.Add("Accept", "application/json");
                var response = client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain"), ct)
                    .Result;
                return await response.Content.ReadAsStringAsync(ct);
            }
    }
}