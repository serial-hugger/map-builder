using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Google.Common.Geometry;
using MapBuilder.Shared.SerializationModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

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
    public ICollection<Node> Nodes { get; } = new List<Node>();
    public ICollection<Way> Ways { get; } = new List<Way>();

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

    public async Task GetNodes()
    {
        
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
                    if (data.Elements[e].Type=="way")
                    {
                        int wayId = (int)data.Elements[e].Id;
                        int nodeAmount = data.Elements[e].Nodes.Count - 1;

                        for (int n = 0; n < data.Elements[e].Nodes.Count; n++)
                        {
                            if ((double)data.Elements[e].Geometry[n].Lat < latHi &&
                                (double)data.Elements[e].Geometry[n].Lat > latLo &&
                                (double)data.Elements[e].Geometry[n].Lon < lngHi &&
                                (double)data.Elements[e].Geometry[n].Lon > lngLo)
                            {
                                if (true)
                                {
                                    Node node = new Node((long)data.Elements[e].Nodes[n],
                                        (double)data.Elements[e].Geometry[n].Lat,
                                        (double)data.Elements[e].Geometry[n].Lon);
                                    node.WayId = wayId;
                                    node.SetProperties(data.Elements[e].Nodes);
                                    Way newWay = new Way(wayId);

                                    newWay.SetProperties(data.Elements[e].Nodes, data.Elements[e].Tags);
                                    if (!string.IsNullOrEmpty(newWay.Type))
                                    {
                                        Map.AddWayAndNode(newWay, node);
                                        Nodes.Add(node);
                                        Ways.Add(newWay);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public string GetInfo()
    {
        string info = $"[CELL] (token:{CellToken}, nodes:{Nodes.Count})";
        foreach (Node node in Nodes)
        {
            info += "\n"+node.GetInfo();
        }

        return info;
    }
}