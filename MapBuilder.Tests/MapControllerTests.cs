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
        var result = _mapController.GetMap(37.5728,-83.7062).Result;
        Assert.IsTrue(result.Contains("[WAY] (id: 477552687, tags: {\"addr:housenumber\":\"210\",\"addr:street\":\"Proctor Bottom Road\",\"building\":\"house\"})"));
    }
}