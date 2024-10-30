using System.Text.Json;
using System.Text.Json.Nodes;
using Google.Common.Geometry;
using MapBuilder.Shared.SerializationModels;

namespace MapBuilder.Shared;

public class Cell
{
    public int Id;
    public string CellToken;
    [NonSerialized]
    public S2Cell MyCell;
    public List<Node> Nodes = new List<Node>();

    public IOSMController OsmController { get; private set; }
    public Map Map { get; private set; }
    public ICellRepository CellRepository { get; private set; }

    public Cell(string cellToken, Map map, IOSMController osmController, ICellRepository cellRepository)
    {
        this.CellToken = cellToken;
        this.MyCell = new S2Cell(S2CellId.FromToken(cellToken));
        OsmController = osmController;
        Map = map;
        CellRepository = cellRepository;
    }

    public async Task GetNodes()
    {
        Nodes.Clear();
        JsonObject? json = await OsmController.GetDataFromBox(MyCell.RectBound.LatLo.Degrees,MyCell.RectBound.LngLo.Degrees,MyCell.RectBound.LatHi.Degrees,MyCell.RectBound.LngHi.Degrees);
        Root? data = JsonSerializer.Deserialize<Root>(json.ToString());
        double latLo = MyCell.RectBound.LatLo.Degrees;
        double lngLo = MyCell.RectBound.LngLo.Degrees;
        double latHi = MyCell.RectBound.LatHi.Degrees;
        double lngHi = MyCell.RectBound.LngHi.Degrees;
        
        Cell cellModel = await CellRepository.GetCellByTokenAsync(CellToken);
        if (cellModel==null)
        {
            if (data != null)
            {
                foreach (Element element in data.elements)
                {
                    int wayId = element.id;

                    bool closed = element.nodes[0] == element.nodes[^1];
                    for (int i = 0; i < element.nodes.Count; i++)
                    {
                        if (element.geometry[i].lat < latHi && element.geometry[i].lat > latLo &&
                            element.geometry[i].lon < lngHi && element.geometry[i].lon > lngLo)
                        {
                            if (i < element.nodes.Count - 1 || !closed)
                            {
                                Node node = new Node(element.nodes[i], element.geometry[i].lat,
                                    element.geometry[i].lon);
                                node.WayId = wayId;
                                node.SetProperties(element.nodes);
                                Way newWay = new Way(wayId);
                                newWay.SetProperties(element.nodes,element.tags);
                                Map.AddWayAndNode(newWay, node);
                                Nodes.Add(node);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            //TODO get cell from database
        }
    }
    public string GetInfo()
    {
        string info = $"[CELL] (token:{CellToken}, nodes:{Nodes.Count})";
        foreach (Node node in Nodes)
        {
            info += "\n"+node.GetInfo();
        }

        return info;
    }
}