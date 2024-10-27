using MapBuilder.Controllers;

namespace MapBuilder.Tests;

[TestClass]
public class OSMControllerTests
{
    private readonly OSMController _osmController = new OSMController();
    private readonly MapBuilder _mapBuilder = new MapBuilder();
    [TestMethod]
    public void GetDataTest()
    {
        var result = _osmController.GetData(37.5667,-83.7243,37.5821,-83.6925).Result;
        Assert.IsTrue(result.ContainsKey("osm3s")&&result.ContainsKey("elements"));
    }
}