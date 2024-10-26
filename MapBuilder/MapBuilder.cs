using System.Text.Json.Nodes;
using Google.Common.Geometry;
using MapBuilder.Controllers;

namespace MapBuilder;

public class MapBuilder
{
    private readonly CellsController _cellsController;
    private readonly OSMController _osmController;
    
    public List<Cell> cells { get; private set; } = new List<Cell>();
    
    public MapBuilder()
    {
        _cellsController = new CellsController();
        _osmController = new OSMController();
    }

    public async Task BuildMap(List<S2CellId> cellIds)
    {
        foreach (S2CellId cellId in cellIds)
        {
            var cell = new Cell(cellId.ToToken());
            await cell.GetNodes();
            cells.Add(cell);
        }
    }

    public string GetInfo()
    {
        string info = "";
        foreach (Cell cell in cells)
        {
            info += "\n"+cell.GetInfo();
        }
        return info;
    }
}