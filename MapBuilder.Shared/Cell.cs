using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Google.Common.Geometry;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json;

namespace MapBuilder.Shared;

public class Cell
{
    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonPropertyName("version")]
    public int GenerationVersion { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    [JsonPropertyName("time")]
    public DateTime GenerationTime { get; set; }
    [JsonPropertyName("token")]
    public string CellToken { get; set; }
    [NonSerialized]
    public S2Cell MyCell;
    [JsonPropertyName("nodes")]
    public ICollection<FeaturePoint> Nodes { get; } = new List<FeaturePoint>();
    [JsonPropertyName("ways")]
    public ICollection<Feature> Ways { get; } = new List<Feature>();

    [NonSerialized] 
    public IOSMController? OsmController;
    [NonSerialized]
    public Map Map;
    [NonSerialized]
    public ICellRepository? CellRepository;

    public Cell(string cellToken, Map map, IOSMController osmController, ICellRepository cellRepository)
    {
        CellToken = cellToken;
        MyCell = new S2Cell(S2CellId.FromToken(cellToken));
        OsmController = osmController;
        Map = map;
        CellRepository = cellRepository;
    }

    private Cell(int id, string cellToken)
    {
        Id = id;
        CellToken = cellToken;
    }
    
    public async Task GetPoints(Func<int,string>? completion,int totalCells,  FeatureSettings? featureSettings)
    {
        FeatureSettings featureSettingsJson;
        string jsonFilepath;
        string jsonContent;
        if (featureSettings == null)
        {
            jsonFilepath = "Settings/featuresettings.json";
            jsonContent = File.ReadAllText(jsonFilepath);
            featureSettingsJson = JsonConvert.DeserializeObject<FeatureSettings>(jsonContent);
        }
        else
        {
            featureSettingsJson = featureSettings;
        }
        Ways.Clear();
        Nodes.Clear();
        if (OsmController != null)
        {
            OSM? data = await OsmController.GetDataFromBox(MyCell.RectBound.LatLo.Degrees,
                    MyCell.RectBound.LngLo.Degrees, MyCell.RectBound.LatHi.Degrees, MyCell.RectBound.LngHi.Degrees);
            
            if (data != null)
            {
                if (CellToken=="886973")
                {
                    Console.WriteLine($"Elements {data.Elements.Count}.");
                    Console.WriteLine($"LatHi {MyCell.RectBound.LatHi.Degrees} LngHi {MyCell.RectBound.LngHi.Degrees} LatLo {MyCell.RectBound.LatLo.Degrees} LngLo {MyCell.RectBound.LngLo.Degrees}");
                }
                for (int e = 0; e < data.Elements.Count; e++)
                {
                    long id = 0;
                    if (data.Elements[e].Type == "way")
                    {
                        id = (int)data.Elements[e].Id;
                    }
                    if (data.Elements[e].Ref!= null && data.Elements[e].Type == "relation")
                    {
                        id = (int)data.Elements[e].Ref;
                    }

                    int pointAmount = 0;

                    if (data.Elements[e].Members!=null)
                    {
                        foreach (var member in data.Elements[e].Members)
                        {
                            id = (long)member.Ref;
                            if (member.Geometry!=null)
                            {
                                pointAmount = member.Geometry.Count;
                                for (int n = 0; n < member.Geometry.Count; n++)
                                {
                                    if (IsWithinBounds(member.Geometry[n]))
                                    {
                                        FeaturePoint featurePoint = new FeaturePoint(member.Geometry[n].Lat,
                                            member.Geometry[n].Lon);
                                        featurePoint.WayId = id;
                                        featurePoint.CellId = Id;
                                        featurePoint.SetProperties(member.Geometry, n==0);
                                        Feature newFeature = new Feature(id);

                                        newFeature.SetProperties(member.Geometry, data.Elements[e].Tags,featureSettingsJson);
                                        AddFeatureAndPoint(newFeature,featurePoint);
                                    }
                                }
                            }
                        }
                    }
                    else if (data.Elements[e].Nodes!=null)
                    {
                        pointAmount = data.Elements[e].Nodes.Count - 1;
                        for (int n = 0; n < data.Elements[e].Nodes.Count; n++)
                        {
                            if (IsWithinBounds(data.Elements[e].Geometry[n]))
                            {
                                FeaturePoint featurePoint = new FeaturePoint(data.Elements[e].Nodes[n],
                                    data.Elements[e].Geometry[n].Lat,
                                    data.Elements[e].Geometry[n].Lon);
                                featurePoint.WayId = id;
                                featurePoint.SetProperties(data.Elements[e].Nodes,n==0);
                                Feature newFeature = new Feature(id);

                                newFeature.SetProperties(data.Elements[e].Geometry, data.Elements[e].Tags,featureSettingsJson);
                                AddFeatureAndPoint(newFeature,featurePoint);
                            }
                        }
                    }
                }
            }
        }

        completion?.Invoke(totalCells);
    }
    private bool IsWithinBounds(Geometry geometry)
    {
        return geometry.Lat < MyCell.RectBound.LatHi.Degrees &&
               geometry.Lat > MyCell.RectBound.LatLo.Degrees &&
               geometry.Lon < MyCell.RectBound.LngHi.Degrees &&
               geometry.Lon > MyCell.RectBound.LngLo.Degrees;
    }

    private void AddFeatureAndPoint(Feature feature, FeaturePoint featurePoint)
    {
        if (!string.IsNullOrEmpty(feature.Type))
        {
            Map.AddWayAndNode(feature, featurePoint);
            if (Nodes.All(point => point.WayId != featurePoint.PointId)||
                Nodes.All(point => point.CellId != featurePoint.CellId)||
                Nodes.All(point => point.PointId != featurePoint.PointId) || 
                Nodes.All(point => point.PointOrder != featurePoint.PointOrder) ||
                Nodes.All(point => point.Lat != featurePoint.Lat) ||
                Nodes.All(point => point.Lng != featurePoint.Lng))
            {
                Nodes.Add(featurePoint);
            }

            if (Ways.All(w => w.WayId != feature.WayId))
            {
                Ways.Add(feature);
            }
        }
    }
    public string GetInfo()
    {
        string info = $"[CELL] (token:{CellToken}, nodes:{Nodes.Count})";
        foreach (FeaturePoint node in Nodes)
        {
            info += "\n"+node.GetInfo();
        }

        return info;
    }
}