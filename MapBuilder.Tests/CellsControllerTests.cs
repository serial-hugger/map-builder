using Google.Common.Geometry;
using MapBuilder.Controllers;

namespace MapBuilder.Tests;

[TestClass]
public class CellsControllerTests
{
    private readonly CellsController _cellsController = new CellsController();
    private readonly MapBuilder _mapBuilder = new MapBuilder();
    [TestMethod]
    public void GetCellsTest()
    {
        var result = _cellsController.GetCells(15,37.5667,-83.7243,37.5821,-83.6925).Result;
        Assert.IsTrue(result.Count>5);
    }
}