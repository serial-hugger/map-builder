using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using MapBuilder.Shared.SerializationModels;

namespace MapBuilder.Shared;

[Serializable]
public class Way
{
    [Key]
    public int Id { get; set; }
    public Int64 WayId { get; set; }
    public int TotalNodes { get; set; }
    public string Type { get; set; }
    public bool Closed { get; set; }
    public bool Filled { get; set; }
    public string Color { get; set; }
    public int CellId { get; set; }
    public Way(Int64 wayId)
    {
        this.WayId  = wayId;
    }

    private Way(int cellId, Int64 wayId)
    {
        this.CellId = cellId;
        this.WayId = wayId;
    }
    public void SetProperties(List<long> nodeIds,Tags tags)
    {
        Type = "";
        Color = "";
        if (nodeIds[0]==nodeIds[^1])
        {
            TotalNodes = nodeIds.Count-1;
            Closed = true;
        }
        else
        {
            TotalNodes = nodeIds.Count;
        }
        if (tags != null)
        {
            if (tags.waterway != null)
            {
                if (tags.waterway == "river")
                {
                    Filled = true;
                    Color = "#0009FF";
                    Type = "water";
                }
            }

            if (tags.water != null)
            {
                if (tags.water == "pond" || tags.water == "lake" || tags.water == "river")
                {
                    Filled = true;
                    Color = "#0009FF";
                    Type = "water";
                }
            }

            if (tags.natural != null)
            {
                if (tags.natural == "water")
                {
                    Filled = true;
                    Color = "#0009FF";
                    Type = "water";
                }
            }

            if (tags.building != null)
            {
                Filled = true;
                Color = "#6D2600";
                Type = "building";
            }

            if (tags.natural != null)
            {
                if (tags.natural == "wood")
                {
                    Filled = true;
                    Color = "#0c7d00";
                    Type = "trees";
                }
            }

            if (tags.highway != null)
            {
                Color = "#4E4E4E";
                Type = "route";
            }
        }
    }
} 