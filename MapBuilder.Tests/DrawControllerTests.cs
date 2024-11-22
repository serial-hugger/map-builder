using MapBuilder.Api.Controllers;
using MapBuilder.Data;
using MapBuilder.Shared;

namespace MapBuilder.Tests;

[TestClass]
public class DrawControllerTests
{
    private static readonly CellsController _cellsController = new CellsController();
    private static readonly OSMController _osmController = new OSMController();
    private static readonly DrawController _drawController = new DrawController();
    private static readonly CellRepository _cellRepository = new CellRepository();
    
    private readonly Map _map = new Map(_cellsController,_osmController,_cellRepository);
    [TestMethod]
    public void InstructionTest()
    {
        var result = _drawController.Instructions(15,37.57240563371417,-83.71679868454393, null).Result;
        Assert.IsTrue(result.Contains("center:37.57137671040393,-83.71592846453665;highs:37.5727740610556,-83.71460005762208;lows:37.56997935975227,-83.71725687145121"));
    }
}