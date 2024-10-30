using System.Runtime.CompilerServices;
using MapBuilder.Shared.SerializationModels;

namespace MapBuilder.Shared;

[Serializable]
public class Way
{
    public Int64? id;
    public int totalNodes;
    public string type = null;
    public bool closed = false;
    public bool filled = false;
    public string color = "#13b101";
    public Way(Int64 id)
    {
        this.id = id;
    }
    public void SetProperties(List<long> nodeIds,Tags tags)
    {
        if (nodeIds[0]==nodeIds[^1])
        {
            closed = true;
        }
        if (tags != null)
        {
            if (tags.waterway != null)
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
    }
} 