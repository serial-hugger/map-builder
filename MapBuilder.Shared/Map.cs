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
            Cell? repoCell = await _cellRepository.GetCellByTokenAsync(cellId.ToToken());
            if (repoCell == null)
            {
                var cell = new Cell(cellId.ToToken(), this, _osmController, _cellRepository);
                cell.CellToken = cellId.ToToken();
                await cell.GetNodes();
                Cells.Add(cell);
                await _cellRepository.AddCell(cell);
            }
            else
            {
                Cells.Add(repoCell);
                foreach (var way in repoCell.Ways)
                {
                    Ways.Add(way);
                }
            }
        }
    }
    public void AddWayAndNode(Way newWay, Node newNode)
    {
        Way foundWay = null;
        foreach (Way way in Ways)
        {
            if (way.WayId == newWay.WayId)
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
            info += $"[WAY] (id: {way.WayId}, type: {way.Type}, closed: {way.Closed})\n";
        }
        foreach (Cell cell in Cells)
        {
            info += "\n"+cell.GetInfo();
        }
        return info;
    }
}