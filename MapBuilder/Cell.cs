using System.Text.Json;
using System.Text.Json.Nodes;
using Google.Common.Geometry;
using MapBuilder.Controllers;
using Newtonsoft.Json;

namespace MapBuilder;

public class Cell
{
    public string cellToken;
    public S2Cell myCell;
    public List<Node> nodes = new List<Node>();
    
    private readonly CellsController _cellsController;
    private readonly OSMController _osmController;

    public Cell(string cellToken)
    {
        this.cellToken = cellToken;
        this.myCell = new S2Cell(S2CellId.FromToken(cellToken));
        _cellsController = new CellsController();
        _osmController = new OSMController();
    }

    public async Task GetNodes()
    {
        nodes.Clear();
        JsonObject? json = await _osmController.GetData(myCell.RectBound.LatLo.Degrees,myCell.RectBound.LngLo.Degrees,myCell.RectBound.LatHi.Degrees,myCell.RectBound.LngHi.Degrees);
        Root? data = JsonConvert.DeserializeObject<Root>(json.ToString());
        double latLo = myCell.RectBound.LatLo.Degrees;
        double lngLo = myCell.RectBound.LngLo.Degrees;
        double latHi = myCell.RectBound.LatHi.Degrees;
        double lngHi = myCell.RectBound.LngHi.Degrees;
        if (data != null)
            foreach (Element element in data.elements)
            {
                int wayId = element.id;
                bool closed = element.nodes[0] == element.nodes[^1];
                for (int i = 0; i < element.nodes.Count; i++)
                {
                    if (element.geometry[i].lat < latHi && element.geometry[i].lat > latLo && element.geometry[i].lon < lngHi && element.geometry[i].lon > lngLo)
                    {
                        if (i < element.nodes.Count - 1 || !closed)
                        {
                            Node node = new Node(element.nodes[i], element.geometry[i].lat, element.geometry[i].lon);
                            node.wayId = wayId;

                            nodes.Add(node);
                        }
                    }
                }
            }
    }
    public string GetInfo()
    {
        string info = $"[CELL] (token:{cellToken}, nodes:{nodes.Count})";
        foreach (Node node in nodes)
        {
            info += "\n"+node.GetInfo();
        }

        return info;
    }
}