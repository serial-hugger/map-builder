using Google.Common.Geometry;
using Microsoft.AspNetCore.Mvc;

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
        var token = S2CellId.FromLatLng(coord).ParentForLevel(10).ToToken();
        S2Cell bigCell = new S2Cell(S2CellId.FromToken(token));
        List<S2CellId> cells = await _cellsController.GetCells(15,bigCell.RectBound.LatLo.Degrees,bigCell.RectBound.LngLo.Degrees,bigCell.RectBound.LatHi.Degrees,bigCell.RectBound.LngHi.Degrees);
        MapBuilder builder = new MapBuilder();
        await builder.BuildMap(cells);
        return builder.GetInfo();
    }
    public MapController()
    {
        _cellsController = new CellsController();
        _osmController = new OSMController();
    }
}