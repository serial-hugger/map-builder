namespace MapBuilder.Shared;

[Serializable]
public class Node
{
    public int Id { get; set; }
    public Int64 NodeId;
    public Int64? WayId;
    public int NodeOrder;
    public double Lat;
    public double Lng;
    public Node(Int64 nodeId, double lat, double lng)
    {
        this.NodeId = nodeId;
        this.Lat = lat;
        this.Lng = lng;
    }

    public void SetProperties(List<long> nodeIds)
    {
        for(int i = 0; i < nodeIds.Count; i++)
        {
            if (nodeIds[i]==NodeId)
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