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
            JToken json = await OsmController.GetDataFromBox(MyCell.RectBound.LatLo.Degrees,MyCell.RectBound.LngLo.Degrees,MyCell.RectBound.LatHi.Degrees,MyCell.RectBound.LngHi.Degrees);
            Root? data = JsonSerializer.Deserialize<Root>(json.ToString());
            double latLo = MyCell.RectBound.LatLo.Degrees;
            double lngLo = MyCell.RectBound.LngLo.Degrees;
            double latHi = MyCell.RectBound.LatHi.Degrees;
            double lngHi = MyCell.RectBound.LngHi.Degrees;
            if (data != null)
            {
                for(int e = 0; e < json["elements"].Count();e++)
                {

                    int wayId = (int)json["elements"][e]["id"];
                    int nodeAmount = json["elements"][e]["nodes"].Count() - 1;

                    bool closed = json["elements"][e]["nodes"][0] == json["elements"][e]["nodes"][nodeAmount];
                    for (int n = 0; n < json["elements"][e]["nodes"].Count(); n++)
                    {
                        if ((double)json["elements"][e]["geometry"][n]["lat"] < latHi && (double)json["elements"][e]["geometry"][n]["lat"] > latLo &&
                            (double)json["elements"][e]["geometry"][n]["lon"] < lngHi && (double)json["elements"][e]["geometry"][n]["lon"] > lngLo)
                        {
                            if (true)
                            {
                                Node node = new Node((long)json["elements"][e]["nodes"][n], (double)json["elements"][e]["geometry"][n]["lat"],
                                    (double)json["elements"][e]["geometry"][n]["lon"]);
                                node.WayId = wayId;
                                node.SetProperties(json["elements"][e]["nodes"]);
                                Way newWay = new Way(wayId);
                                
                                JToken? tags = json["elements"][e]["tags"];
                                
                                newWay.SetProperties(json["elements"][e]["nodes"],tags);
                                if (newWay.Type!="" && newWay.Type != null)
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