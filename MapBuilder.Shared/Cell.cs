using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Google.Common.Geometry;
using MapBuilder.Shared.SerializationModels;
using Microsoft.EntityFrameworkCore;

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
            JsonObject? json = await OsmController.GetDataFromBox(MyCell.RectBound.LatLo.Degrees,MyCell.RectBound.LngLo.Degrees,MyCell.RectBound.LatHi.Degrees,MyCell.RectBound.LngHi.Degrees);
            Root? data = JsonSerializer.Deserialize<Root>(json.ToString());
            double latLo = MyCell.RectBound.LatLo.Degrees;
            double lngLo = MyCell.RectBound.LngLo.Degrees;
            double latHi = MyCell.RectBound.LatHi.Degrees;
            double lngHi = MyCell.RectBound.LngHi.Degrees;
            if (data != null)
            {
                foreach (Element element in data.elements)
                {
                    int wayId = element.id;

                    bool closed = element.nodes[0] == element.nodes[^1];
                    for (int i = 0; i < element.nodes.Count; i++)
                    {
                        if (element.geometry[i].lat < latHi && element.geometry[i].lat > latLo &&
                            element.geometry[i].lon < lngHi && element.geometry[i].lon > lngLo)
                        {
                            if (i < element.nodes.Count - 1 || !closed)
                            {
                                Node node = new Node(element.nodes[i], element.geometry[i].lat,
                                    element.geometry[i].lon);
                                node.WayId = wayId;
                                node.SetProperties(element.nodes);
                                Way newWay = new Way(wayId);
                                newWay.SetProperties(element.nodes,element.tags);
                                if (newWay.Type!="")
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