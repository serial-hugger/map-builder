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
    public OSMController()
    {
        _context = new CellContext();
        _cellRepository = new CellRepository();
    }
    ///osm/getdata/40.95876296470057/-74.28306490222923/40.44841428528941/-72.78636592806494
    [HttpGet("{action}/{latitudeLo}/{longitudeLo}/{latitudeHi}/{longitudeHi}")]
    public async Task<OSM?> GetDataFromBox(double latitudeLo, double longitudeLo, double latitudeHi, double longitudeHi)
    {
        string apiUrl = "https://overpass-api.de/api/interpreter";
        string query = $"[out:json][maxsize:1073741824][timeout:900];node({latitudeLo},{longitudeLo},{latitudeHi},{longitudeHi});<<;out geom;";

        using (var client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromMinutes(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            HttpResponseMessage response= client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain")).Result;

            var jsonString = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"{latitudeLo}/{longitudeLo}/{latitudeHi}/{longitudeHi}");
            return JsonConvert.DeserializeObject<OSM>(jsonString);
        }
    }
    [HttpGet("{action}/{cellToken}")]
    public async Task<OSM?> GetData(string cellToken)
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
                var response = client.PostAsync(apiUrl, new StringContent(query, Encoding.UTF8, "text/plain")).Result;
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OSM>(jsonString);
            }
        }
        else
        {
            var jsonString = JsonSerializer.Serialize(cellModel);
            return JsonSerializer.Deserialize<OSM>(jsonString);
        }
    }
}