using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace MapBuilder.Shared;

[Serializable]
public class Node
{
    [Key]
    public int Id { get; set; }
    [JsonIgnore]
    public long NodeId { get; set; }

    public long? WayId { get; set; }
    public int NodeOrder { get; set; }
    public double Lat { get; set; }
    public double Lng { get; set; }
    [JsonIgnore]
    public int CellId { get; set; }
    public Node(long nodeId, double lat, double lng)
    {
        this.NodeId = nodeId;
        this.Lat = lat;
        this.Lng = lng;
    }
    private Node(int cellId, long nodeId, double lat, double lng, int nodeOrder)
    {
        this.CellId = cellId;
        this.NodeId = nodeId;
        this.Lat = lat;
        this.Lng = lng;
        this.NodeOrder = nodeOrder;
    }

    public void SetProperties(List<long> nodeIds)
    {
        for(int i = 0; i < nodeIds.Count(); i++)
        {
            if ((long)nodeIds[i]==NodeId)
            {
                NodeOrder = i;
                return;
            }
        }
    }

    public string GetInfo()
    {
        return $"   [NODE] (id:{NodeId}, lat:{Lat}, lng:{Lng}, wayId:{WayId})";
    }
}