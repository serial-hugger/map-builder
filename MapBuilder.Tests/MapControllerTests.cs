using MapBuilder.Controllers;

namespace MapBuilder.Tests;

[TestClass]
public class MapControllerTests
{
    private readonly MapController _mapController = new MapController();
    private readonly MapBuilder _mapBuilder = new MapBuilder();
    [TestMethod]
    public void GetMapTest()
    {
        var result = _mapController.GetMap(37.5728,-83.7062).Result;
        Assert.IsTrue(result.Contains("[WAY] (id: 477552687, tags: {\"addr:housenumber\":\"210\",\"addr:street\":\"Proctor Bottom Road\",\"building\":\"house\"})"));
    }
}