using Google.Common.Geometry;
using MapBuilder.Api;
using MapBuilder.Api.Controllers;
using MapBuilder.Data;
using MapBuilder.Shared;

namespace MapBuilder.Tests;

[TestClass]
public class CellsControllerTests
{
    private static readonly CellsController _cellsController = new CellsController();
    private static readonly OSMController _osmController = new OSMController();
    private static readonly MapController _mapController = new MapController();
    private static readonly CellRepository _cellRepository = new CellRepository();
    
    private readonly Map _map = new Map(_cellsController,_osmController,_cellRepository);
    [TestMethod]
    public void GetCellsTest()
    {
        var result = _cellsController.GetCells(15,37.5667,-83.7243,37.5821,-83.6925).Result;
        Assert.IsTrue(result.Count>5);
    }
}