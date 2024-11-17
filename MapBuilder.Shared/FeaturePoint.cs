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
    public int Id { get; set; }
    [JsonIgnore]
    public long? PointId { get; set; }

    public long? WayId { get; set; }
    public int PointOrder { get; set; }
    public double Lat { get; set; }
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

    public void SetProperties(List<long> nodeIds)
    {
        for(int i = 0; i < nodeIds.Count(); i++)
        {
            if ((long)nodeIds[i]==PointId)
            {
                PointOrder = i;
                return;
            }
        }
    }
    public void SetProperties(List<Geometry> geometries)
    {
        for(int i = 0; i < geometries.Count; i++)
        {
            if (geometries[i].Lat.ToString()==Lat.ToString() && geometries[i].Lon.ToString()==Lng.ToString())
            {
                PointOrder = i;
                return;
            }
        }
    }

    public string GetInfo()
    {
        return $"   [NODE] (id:{PointId}, lat:{Lat}, lng:{Lng}, wayId:{WayId})";
    }
}