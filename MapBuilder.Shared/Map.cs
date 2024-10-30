using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Common.Geometry;
using MapBuilder.Shared;

namespace MapBuilder.Shared;

public class Map
{
    private readonly ICellsController _cellsController;
    private readonly IOSMController _osmController;
    private readonly ICellRepository _cellRepository;
    
    public List<Cell> Cells = new List<Cell>();
    public List<Way> Ways = new List<Way>();
    public List<Node> Nodes = new List<Node>();
    
    public Map(ICellsController cellsController, IOSMController osmController, ICellRepository cellRepository)
    {
        _cellsController = cellsController;
        _osmController = osmController;
        _cellRepository = cellRepository;
    }

    public async Task BuildMap(List<S2CellId> cellIds)
    {
        Ways.Clear();
        foreach (S2CellId cellId in cellIds)
        {
            var cell = new Cell(cellId.ToToken(),this,_osmController,_cellRepository);
            cell.CellToken = cellId.ToToken();
            await cell.GetNodes();
            Cells.Add(cell);
        }
    }
    public void AddWayAndNode(Way newWay, Node newNode)
    {
        Way foundWay = null;
        foreach (Way way in Ways)
        {
            if (way.id == newWay.id)
            {
                foundWay = way;
            }
        }
        if (foundWay == null)
        {
            foundWay = newWay;
            Ways.Add(foundWay);
        }
        Nodes.Add(newNode);
    }

    public string GetInfo()
    {
        string info = "";
        var options = new JsonSerializerOptions();
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        foreach (Way way in Ways)
        {
            info += $"[WAY] (id: {way.id}, type: {way.type}, closed: {way.closed})\n";
        }
        foreach (Cell cell in Cells)
        {
            info += "\n"+cell.GetInfo();
        }
        return info;
    }
}