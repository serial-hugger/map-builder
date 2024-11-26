using MapBuilder.Api.Controllers;
using MapBuilder.Data;
using MapBuilder.Shared;

namespace MapBuilder.Tests;

[TestClass]
public class DrawInstructerTests
{
    private static readonly CellsController CellsController = new CellsController();
    private static readonly OSMController OsmController = new OSMController();
    private static readonly DrawInstructer DrawInstructer = new DrawInstructer(null,null);
    private static readonly CellRepository CellRepository = new CellRepository();
    
    private readonly Map _map = new Map(CellsController,OsmController,CellRepository);
    [TestMethod]
    public void InstructionTest()
    {
        var result = DrawInstructer.Instructions(15,37.57240563371417,-83.71679868454393, null, new CancellationToken()).Result;
        Assert.IsTrue(result.Contains("center:37.57137671040393,-83.71592846453665;highs:37.5727740610556,-83.71460005762208;lows:37.56997935975227,-83.71725687145121"));
    }
}