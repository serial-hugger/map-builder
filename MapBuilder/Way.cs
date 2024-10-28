namespace MapBuilder;

[Serializable]
public class Way
{
    public Int64? id;
    [NonSerialized]
    public List<Int64> nodeIds = new List<Int64>();
    public List<Node> nodes = new List<Node>();
    public Tags tags = new Tags();
    public bool closed = false;
    public bool filled = false;
    public Way(Int64 id,List<Int64> nodeIds, Tags tags)
    {
        this.id = id;
        this.tags = tags;
        this.nodeIds = nodeIds;
    }

    public void OrderNodes()
    {
        List<Node> tempNodes = nodes.ToList();
        List<Node> newNodes = new List<Node>();
        foreach (var nodeId in nodeIds)
        {
            foreach(var node in tempNodes)
            {
                if (node.id == nodeId)
                {
                    newNodes.Add(node);
                    break;
                }
            }
        }
        nodes = newNodes.ToList();
    }
}