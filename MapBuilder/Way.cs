namespace MapBuilder;

public class Way
{
    public Int64? id;
    public List<Int64> nodeIds = new List<Int64>();
    public Tags tags = new Tags();
    public Way(Int64 id,List<Int64> nodeIds, Tags tags)
    {
        this.id = id;
        this.tags = tags;
        this.nodeIds = nodeIds;
    }
}