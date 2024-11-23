using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Google.Common.Geometry;
using MapBuilder.Shared.SerializationModels;

namespace MapBuilder.Shared;

public class Cell
{
    [Key]
    public int Id { get; set; }
    [JsonIgnore]
    public int GenerationVersion { get; set; }
    [JsonIgnore]
    public DateTime GenerationTime { get; set; }
    public string CellToken { get; set; }
    [NonSerialized]
    public S2Cell MyCell;
    public ICollection<FeaturePoint> Nodes { get; } = new List<FeaturePoint>();
    public ICollection<Feature> Ways { get; } = new List<Feature>();

    [NonSerialized] 
    public IOSMController? OsmController = null;
    [NonSerialized]
    public Map Map;
    [NonSerialized]
    public ICellRepository? CellRepository = null;

    public Cell(string cellToken, Map map, IOSMController osmController, ICellRepository cellRepository)
    {
        this.CellToken = cellToken;
        this.MyCell = new S2Cell(S2CellId.FromToken(cellToken));
        OsmController = osmController;
        Map = map;
        CellRepository = cellRepository;
    }

    private Cell(int id, string cellToken)
    {
        Id = id;
        CellToken = cellToken;
    }

    public async Task GetPoints(Func<int,string>? completion,int totalCells)
    {
        Ways.Clear();
        Nodes.Clear();
        if (OsmController != null)
        {
            OSM data = await OsmController.GetDataFromBox(MyCell.RectBound.LatLo.Degrees,MyCell.RectBound.LngLo.Degrees,MyCell.RectBound.LatHi.Degrees,MyCell.RectBound.LngHi.Degrees);
            double latLo = MyCell.RectBound.LatLo.Degrees;
            double lngLo = MyCell.RectBound.LngLo.Degrees;
            double latHi = MyCell.RectBound.LatHi.Degrees;
            double lngHi = MyCell.RectBound.LngHi.Degrees;
            if (data != null)
            {
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
                                    if ((double)member.Geometry[n].Lat < latHi &&
                                        (double)member.Geometry[n].Lat > latLo &&
                                        (double)member.Geometry[n].Lon < lngHi &&
                                        (double)member.Geometry[n].Lon > lngLo)
                                    {
                                        if (true)
                                        {
                                            FeaturePoint featurePoint = new FeaturePoint((double)member.Geometry[n].Lat,
                                                (double)member.Geometry[n].Lon);
                                            featurePoint.WayId = id;
                                            featurePoint.CellId = Id;
                                            featurePoint.SetProperties(member.Geometry, n==0);
                                            Feature newFeature = new Feature(id);

                                            newFeature.SetProperties(member.Geometry, data.Elements[e].Tags);
                                            if (!string.IsNullOrEmpty(newFeature.Type))
                                            {
                                                Map.AddWayAndNode(newFeature, featurePoint);
                                                if (Nodes.All(point => point.WayId != featurePoint.WayId) || 
                                                    Nodes.All(point => point.CellId != featurePoint.CellId) || 
                                                    Nodes.All(point => point.PointId != featurePoint.PointId) || 
                                                    Nodes.All(point => point.PointOrder != featurePoint.PointOrder))
                                                {
                                                    Nodes.Add(featurePoint);
                                                }
                                                if (Ways.All(w => w.WayId != id))
                                                {
                                                    Ways.Add(newFeature);
                                                }
                                            }
                                        }
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
                            if ((double)data.Elements[e].Geometry[n].Lat < latHi &&
                                (double)data.Elements[e].Geometry[n].Lat > latLo &&
                                (double)data.Elements[e].Geometry[n].Lon < lngHi &&
                                (double)data.Elements[e].Geometry[n].Lon > lngLo)
                            {
                                if (true)
                                {
                                    FeaturePoint featurePoint = new FeaturePoint((long)data.Elements[e].Nodes[n],
                                        (double)data.Elements[e].Geometry[n].Lat,
                                        (double)data.Elements[e].Geometry[n].Lon);
                                    featurePoint.WayId = id;
                                    featurePoint.SetProperties(data.Elements[e].Nodes,n==0);
                                    Feature newFeature = new Feature(id);

                                    newFeature.SetProperties(data.Elements[e].Geometry, data.Elements[e].Tags);
                                    if (!string.IsNullOrEmpty(newFeature.Type))
                                    {
                                        Map.AddWayAndNode(newFeature, featurePoint);
                                        if (Nodes.All(point => point.WayId != featurePoint.PointId)||
                                            Nodes.All(point => point.CellId != featurePoint.CellId)||
                                            Nodes.All(point => point.PointId != featurePoint.PointId) || 
                                            Nodes.All(point => point.PointOrder != featurePoint.PointOrder))
                                        {
                                            Nodes.Add(featurePoint);
                                        }

                                        if (Ways.All(w => w.WayId != id))
                                        {
                                            Ways.Add(newFeature);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        completion?.Invoke(totalCells);
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