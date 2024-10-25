namespace MapBuilder;

public class Node
{
    public int nodeId;
    public double lat;
    public double lon;
    public int? wayId;
    public int? nextNodeId;
    public int? prevNodeId;

    public Node(int nodeId, double lat, double lon)
    {
        this.nodeId = nodeId;
        this.lat = lat;
        this.lon = lon;
    }
}