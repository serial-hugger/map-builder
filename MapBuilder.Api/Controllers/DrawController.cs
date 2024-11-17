using System.Drawing;
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
        var token = S2CellId.FromLatLng(coord).ParentForLevel(10).ToToken();
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
                newNodes.Add(node);
            }
        }
        string instructions = "";

        double centerLat = bigCell.RectBound.Center.LatDegrees;
        double centerLng = bigCell.RectBound.Center.LngDegrees;
        double lngHi = bigCell.RectBound.LngHi.Degrees;
        double latHi = bigCell.RectBound.LatHi.Degrees;
        double lngLo = bigCell.RectBound.LngLo.Degrees;
        double latLo = bigCell.RectBound.LatLo.Degrees;
        float latRange = (float)(latHi - latLo);
        float lngRange = (float)(lngHi - lngLo);
        instructions = AddInstructions(instructions,"center",$"{centerLat},{centerLng}");
        instructions = AddInstructions(instructions,"highs",$"{latHi},{lngHi}");
        instructions = AddInstructions(instructions,"lows",$"{latLo},{lngLo}");
        foreach (Feature way in newWays)
        {
            instructions = AddInstructions(instructions,"type",way.Type);
            instructions = AddInstructions(instructions,"filled",way.Filled.ToString());
            instructions = AddInstructions(instructions,"closed",way.Closed.ToString());
            List<FeaturePoint> nodes = new List<FeaturePoint>();
            foreach (FeaturePoint node in newNodes)
            {
                if (node.WayId==way.WayId)
                {
                    nodes.Add(node);
                }
            }

            var orderedNodes = nodes.OrderBy(n => n.PointOrder);

            string nodeLocations = "";
            foreach (FeaturePoint node in orderedNodes)
            {
                if (nodeLocations!="")
                {
                    nodeLocations += ",";
                }
                nodeLocations += $"{(((node.Lat-latLo)/latRange)-0.5f).ToString("N10")},{(((node.Lng-lngLo)/lngRange)-0.5f).ToString("N10")}";
            }
            if (nodes.Any())
            {
                instructions = AddInstructions(instructions,"points",nodeLocations);
            }
        }
        return instructions;
    }
    public string AddInstructions(string currentInstructions,string key, string value)
    {
        if (currentInstructions!="")
        {
            currentInstructions += ";";
        }
        currentInstructions += key+":"+value;
        return currentInstructions;
    }
    public DrawController()
    {
        _cellsController = new CellsController();
        _osmController = new OSMController();
        _cellRepository = new CellRepository();
    }
}