using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Common.Geometry;
using MapBuilder.Data;
using MapBuilder.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MapBuilder.Api.Controllers;

[ApiController]
[Route("{controller}")]
public class DrawController : ControllerBase, IDrawController
{
    private readonly CellsController _cellsController;
    private readonly OSMController _osmController;
    private readonly CellRepository _cellRepository;
    
    [HttpGet("{action}/{latitude}/{longitude}")]
    public async Task<string> Instructions(double latitude, double longitude)
    {
        var coord = S2LatLng.FromDegrees(latitude, longitude);
        var token = S2CellId.FromLatLng(coord).ParentForLevel(14).ToToken();
        var bigCell = new S2Cell(S2CellId.FromToken(token));
        var cells = await _cellsController.GetCells(15,bigCell.RectBound.LatLo.Degrees,bigCell.RectBound.LngLo.Degrees,bigCell.RectBound.LatHi.Degrees,bigCell.RectBound.LngHi.Degrees);
        var map = new Map(_cellsController,_osmController,_cellRepository);
        await map.BuildMap(cells);
        List<Cell> newCells = new List<Cell>();
        List<Way> newWays = new List<Way>();
        List<Node> newNodes = new List<Node>();
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
                if (newNodes.All(n => n.NodeId != node.NodeId))
                {
                    newNodes.Add(node);
                }
            }
        }
        string instructions = "";

        double centerLat = bigCell.RectBound.Center.LatDegrees;
        double centerLng = bigCell.RectBound.Center.LngDegrees;
        instructions += $"center:{centerLat}:{centerLng}";
        foreach (Way way in newWays)
        {
            if (instructions!="")
            {
                instructions += ";";
            }
            instructions += $"waydraw:{way.Type}";
            List<Node> nodes = new List<Node>();
            foreach (Node node in newNodes)
            {
                if (node.WayId==way.WayId)
                {
                    nodes.Add(node);
                }
            }
            nodes.OrderBy(n => n.NodeOrder);
            if (nodes.Any())
            {
                instructions += ";nodes";
            }
            foreach (Node node in nodes)
            {
                instructions += $":{(node.Lat-centerLat).ToString("N10")}:{(node.Lng-centerLng).ToString("N10")}";
            }
        }
        return instructions;
    }
    public DrawController()
    {
        _cellsController = new CellsController();
        _osmController = new OSMController();
        _cellRepository = new CellRepository();
    }
}