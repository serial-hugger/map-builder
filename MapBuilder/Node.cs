namespace MapBuilder;

public class Node
{
    public Int64 nodeId;
    public double lat;
    public double lng;
    public int? wayId;
    public int? nextNodeId;
    public int? prevNodeId;

    public Node(Int64 nodeId, double lat, double lng)
    {
        this.nodeId = nodeId;
        this.lat = lat;
        this.lng = lng;
    }

    public string GetInfo()
    {
        return $"[NODE] (id:{nodeId}, lat:{lat}, lng:{lng})";
    }
}