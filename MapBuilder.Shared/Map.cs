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
    public List<Feature> Ways = new List<Feature>();
    public List<FeaturePoint> Nodes = new List<FeaturePoint>();
    
    public Map(ICellsController cellsController, IOSMController osmController, ICellRepository cellRepository)
    {
        _cellsController = cellsController;
        _osmController = osmController;
        _cellRepository = cellRepository;
    }

    public async Task BuildMap(List<S2CellId> cellIds, Func<int,string>? completion, CancellationToken ct)
    {
        string jsonFilepath = "Settings/featuresettings.json";
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
                await cell.GetPoints(completion,cellIds.Count, ct);
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
                await repoCell.GetPoints(completion,cellIds.Count,ct);
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
                
                completion?.Invoke(cellIds.Count);
            }
        }
    }
    public void AddWayAndNode(Feature newFeature, FeaturePoint newFeaturePoint)
    {
        Feature? foundFeature = null;
        foreach (Feature way in Ways)
        {
            if (way.WayId == newFeature.WayId)
            {
                foundFeature = way;
            }
        }
        if (foundFeature == null)
        {
            foundFeature = newFeature;
            Ways.Add(foundFeature);
        }
        Nodes.Add(newFeaturePoint);
    }

    public string GetInfo()
    {
        string info = "";
        var options = new JsonSerializerSettings();
        options.NullValueHandling = NullValueHandling.Ignore;
        foreach (Feature way in Ways)
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