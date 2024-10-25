using Google.Common.Geometry;
using Microsoft.AspNetCore.Mvc;

namespace MapBuilder.Controllers;

[ApiController]
[Route("{controller}")]
public class CellsController : ControllerBase
{
    [HttpGet("{action}/{level}/{latitude1}/{longitude1}/{latitude2}/{longitude2}")]
    public async Task<string> GetCells(int level, double latitude1, double longitude1, double latitude2, double longitude2)
    {
        S2RegionCoverer cov = new S2RegionCoverer();
        cov.MaxLevel = level; // Set the maximum level
        cov.MinLevel = level; // Set the minimum level
        ///getcells/15/37.7749/-122.4194/37.7858/-122.3864
        S2LatLngRect regionRect = new S2LatLngRect(
            new S2LatLng(S1Angle.FromDegrees(latitude1), S1Angle.FromDegrees(longitude1)), // Bottom-left corner
            new S2LatLng(S1Angle.FromDegrees(latitude2), S1Angle.FromDegrees(longitude2)) // Top-right corner
        );

        List<S2CellId> cells = new List<S2CellId>();

        cov.GetCovering(regionRect, cells);
        string ids = "";
        foreach (S2CellId cell in cells)
        {
            ids += cell.ToToken() + ", ";
        }

        return ids;
    }
}