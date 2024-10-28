using System.Runtime.CompilerServices;

namespace MapBuilder;

[Serializable]
public class Way
{
    public Int64? id;
    public string type = null;
    [NonSerialized]
    public List<Int64> nodeIds = new List<Int64>();
    public List<Node> nodes = new List<Node>();
    [NonSerialized]
    public Tags tags = new Tags();
    public bool closed = false;
    public bool filled = false;
    public string color = "#13b101";
    public Way(Int64 id,List<Int64> nodeIds, Tags tags)
    {
        this.id = id;
        this.tags = tags;
        this.nodeIds = nodeIds;
    }

    public void OrderNodes()
    {
        if (nodeIds[0]==nodeIds[^1])
        {
            closed = true;
        }

        if (tags!=null)
        {
            if (tags.waterway!=null)
            {
                if (tags.waterway == "river")
                {
                    filled = true;
                    color = "#0009FF";
                    type = "water";
                }
            }

            if (tags.water != null)
            {
                if (tags.water == "pond" || tags.water == "lake" || tags.water == "river")
                {
                    filled = true;
                    color = "#0009FF";
                    type = "water";
                }
            }

            if (tags.natural != null)
            {
                if (tags.natural == "water")
                {
                    filled = true;
                    color = "#0009FF";
                    type = "water";
                }
            }

            if (tags.building != null)
            {
                filled = true;
                color = "#6D2600";
                type = "building";
            }

            if (tags.natural != null)
            {
                if (tags.natural == "wood")
                {
                    filled = true;
                    color = "#0c7d00";
                    type = "trees";
                }
            }

            if (tags.highway != null)
            {
                color = "#4E4E4E";
                type = "route";
            }
        }

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