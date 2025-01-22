using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using MapBuilder.Shared.SerializationModels;
using Newtonsoft.Json.Linq;

namespace MapBuilder.Shared;

[Serializable]
public class FeaturePoint
{
    [Key]
    [JsonIgnore]
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonIgnore]
    public long? PointId { get; set; }

    [JsonPropertyName("way")]
    public long? WayId { get; set; }
    [JsonPropertyName("order")]
    public int PointOrder { get; set; }
    [JsonPropertyName("lat")]
    public double Lat { get; set; }
    [JsonPropertyName("lng")]
    public double Lng { get; set; }
    [JsonIgnore]
    public int CellId { get; set; }
    public FeaturePoint(long nodeId, double lat, double lng)
    {
        this.PointId = nodeId;
        this.Lat = lat;
        this.Lng = lng;
    }
    public FeaturePoint(double lat, double lng)
    {
        this.Lat = lat;
        this.Lng = lng;
    }
    private FeaturePoint(int cellId, long nodeId, double lat, double lng, int pointOrder)
    {
        this.CellId = cellId;
        this.PointId = nodeId;
        this.Lat = lat;
        this.Lng = lng;
        this.PointOrder = pointOrder;
    }

    public void SetProperties(List<long> nodeIds, bool isFirst)
    {
        for(int i = 0; i < nodeIds.Count(); i++)
        {
            if ((long)nodeIds[i]==PointId)
            {
                if (i>0||isFirst)
                {
                    PointOrder = i;
                    return;
                }
            }
        }
    }
    public void SetProperties(List<Geometry> geometries, bool isFirst)
    {
        for(int i = 0; i < geometries.Count; i++)
        {
            if (geometries[i].Lat.ToString()==Lat.ToString() && geometries[i].Lon.ToString()==Lng.ToString())
            {
                if (i > 0 || isFirst)
                {
                    PointOrder = i;
                    PointId = long.Parse(WayId.ToString() + PointOrder.ToString());
                    return;
                }
            }
        }
    }

    public string GetInfo()
    {
        return $"   [NODE] (id:{PointId}, lat:{Lat}, lng:{Lng}, wayId:{WayId})";
    }
}