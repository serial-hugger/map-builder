using Google.Common.Geometry;
using MapBuilder.Shared;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json;

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
        string jsonFilepath = "Settings/settings.json";
        string jsonContent = File.ReadAllText(jsonFilepath);
        Settings settingsJson = JsonConvert.DeserializeObject<Settings>(jsonContent);
        int generationVersion = (int)settingsJson.GenerationVersion;
        foreach (S2CellId cellId in cellIds)
        {
            Cell? repoCell = await _cellRepository.GetCellByTokenAsync(cellId.ToToken());
            if (repoCell == null)
            {
                var cell = new Cell(cellId.ToToken(), this, _osmController, _cellRepository);
                cell.GenerationVersion = generationVersion;
                cell.GenerationTime = DateTime.Now.ToUniversalTime();
                cell.CellToken = cellId.ToToken();
                await cell.GetNodes();
                Cells.Add(cell);
                await _cellRepository.AddCell(cell);
            }
            else if (repoCell.GenerationVersion < generationVersion || repoCell.GenerationTime < DateTime.Now.ToUniversalTime()-TimeSpan.FromDays(30))
            {
                repoCell.OsmController = _osmController;
                repoCell.CellRepository = _cellRepository;
                repoCell.Map = this;
                repoCell.CellToken = cellId.ToToken();
                repoCell.MyCell = new S2Cell(S2CellId.FromToken(cellId.ToToken()));
                repoCell.GenerationVersion = generationVersion;
                repoCell.GenerationTime = DateTime.Now.ToUniversalTime();
                await repoCell.GetNodes();
                Cells.Add(repoCell);
                await _cellRepository.UpdateCell(repoCell);
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
        var options = new JsonSerializerSettings();
        options.NullValueHandling = NullValueHandling.Ignore;
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