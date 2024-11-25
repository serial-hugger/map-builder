using MapBuilder.Api;
using MapBuilder.Api.Controllers;
using MapBuilder.Data;
using MapBuilder.Shared;

namespace MapBuilder.Tests;

[TestClass]
public class MapControllerTests
{
    private static readonly CellsController _cellsController = new CellsController();
    private static readonly OSMController _osmController = new OSMController();
    private static readonly MapController _mapController = new MapController();
    private static readonly CellRepository _cellRepository = new CellRepository();
    
    private readonly Map _map = new Map(_cellsController,_osmController,_cellRepository);
    [TestMethod]
    public void GetMapTest()
    {
        var result = _mapController.GetMap(15,37.57240563371417,-83.71679868454393,null, new CancellationToken()).Result;
        Assert.IsTrue(result.Contains("\"WayId\":477552687,\"TotalPoints\":9,\"Type\":\"building\""));
    }
}