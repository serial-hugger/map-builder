using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Common.Geometry;
using MapBuilder.Data;
using MapBuilder.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MapBuilder.Api.Controllers;

[ApiController]
[Route("{controller}")]
public class MapController : ControllerBase, IMapController
{
    private readonly CellsController _cellsController;
    private readonly OSMController _osmController;
    private readonly CellRepository _cellRepository;
    
    [HttpGet("{action}/{level}/{latitude}/{longitude}")]
    public async Task<string> GetMap(int level, double latitude, double longitude)
    {
        var coord = S2LatLng.FromDegrees(latitude, longitude);
        var token = S2CellId.FromLatLng(coord).ParentForLevel(level).ToToken();
        var bigCell = new S2Cell(S2CellId.FromToken(token));
        var cells = await _cellsController.GetCells(15,bigCell.RectBound.LatLo.Degrees,bigCell.RectBound.LngLo.Degrees,bigCell.RectBound.LatHi.Degrees,bigCell.RectBound.LngHi.Degrees);
        var map = new Map(_cellsController,_osmController,_cellRepository);
        await map.BuildMap(cells);
        List<Cell> newCells = new List<Cell>();
        List<Feature> newWays = new List<Feature>();
        List<FeaturePoint> newNodes = new List<FeaturePoint>();
        foreach (var cell in map.Cells)
        {
            newCells.Add(cell);
            foreach (var way in cell.Ways)
            {
                if (newWays.All(w => w.WayId != way.WayId))
                {
                    newWays.Add(way);
                }
            }
            foreach (var node in cell.Nodes)
            {
                if (newNodes.All(n => n.PointId != node.PointId))
                {
                    newNodes.Add(node);
                }
            }
        }
        var options = new JsonSerializerOptions();
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        return JsonSerializer.Serialize(newCells,options);
    }
    public MapController()
    {
        _cellsController = new CellsController();
        _osmController = new OSMController();
        _cellRepository = new CellRepository();
    }
}