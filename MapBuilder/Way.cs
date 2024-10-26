namespace MapBuilder;

public class Way
{
    public Int64? id;
    public List<Int64> nodeIds = new List<Int64>();
    public Tags tags = new Tags();
    public Way(Int64 nodeId, Tags tags)
    {
        this.id = nodeId;
        this.tags = tags;
    }
}