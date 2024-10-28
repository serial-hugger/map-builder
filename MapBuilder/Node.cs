namespace MapBuilder;

[Serializable]
public class Node
{
    [NonSerialized]
    public Int64 id;
    public double lat;
    public double lng;
    [NonSerialized]
    public Int64? wayId;

    public Node(Int64 nodeId, double lat, double lng)
    {
        this.id = nodeId;
        this.lat = lat;
        this.lng = lng;
    }

    public string GetInfo()
    {
        return $"   [NODE] (id:{id}, lat:{lat}, lng:{lng}, wayId:{wayId})";
    }
}