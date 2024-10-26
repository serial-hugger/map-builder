using System.Text.Json.Nodes;
using Google.Common.Geometry;
using MapBuilder.Controllers;
using Newtonsoft.Json;

namespace MapBuilder;

public class MapBuilder
{
    private readonly CellsController _cellsController;
    private readonly OSMController _osmController;
    
    public List<Cell> cells { get; private set; } = new List<Cell>();
    public List<Way> ways { get; private set; } = new List<Way>();
    
    public MapBuilder()
    {
        _cellsController = new CellsController();
        _osmController = new OSMController();
    }

    public async Task BuildMap(List<S2CellId> cellIds)
    {
        ways.Clear();
        foreach (S2CellId cellId in cellIds)
        {
            var cell = new Cell(cellId.ToToken(),this);
            await cell.GetNodes();
            cells.Add(cell);
        }
    }

    public void AddWay(Way newWay)
    {
        bool found = false;
        foreach (Way way in ways)
        {
            if (way.id == newWay.id)
            {
                found = true;
            }
        }
        if (!found)
        {
            ways.Add(newWay);
        }
    }

    public string GetInfo()
    {
        string info = "";
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        foreach (Way way in ways)
        {
            info += $"[WAY] (id: {way.id}, tags: {JsonConvert.SerializeObject(way.tags,settings)})\n";
        }
        foreach (Cell cell in cells)
        {
            info += "\n"+cell.GetInfo();
        }
        return info;
    }
}