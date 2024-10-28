using System.Text.Json.Serialization;
using Google.Common.Geometry;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MapBuilder.Controllers;

[ApiController]
[Route("{controller}")]
public class MapController : ControllerBase
{
    private readonly CellsController _cellsController;
    private readonly OSMController _osmController;
    
    [HttpGet("{action}/{latitude}/{longitude}")]
    public async Task<string> GetMap(double latitude, double longitude)
    {
        var coord = S2LatLng.FromDegrees(latitude, longitude);
        var token = S2CellId.FromLatLng(coord).ParentForLevel(14).ToToken();
        var bigCell = new S2Cell(S2CellId.FromToken(token));
        var cells = await _cellsController.GetCells(15,bigCell.RectBound.LatLo.Degrees,bigCell.RectBound.LngLo.Degrees,bigCell.RectBound.LatHi.Degrees,bigCell.RectBound.LngHi.Degrees);
        var builder = new Map();
        await builder.BuildMap(cells);
        List<Way> newWays = new List<Way>();
        foreach(var way in builder.ways)
        {
            if (way.type != null)
            {
                newWays.Add(way);
            }
        }
        var options = new JsonSerializerSettings();
        options.NullValueHandling = NullValueHandling.Ignore;
        return JsonConvert.SerializeObject(newWays,options);
    }
    public MapController()
    {
        _cellsController = new CellsController();
        _osmController = new OSMController();
    }
}