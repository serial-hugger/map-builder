using Google.Common.Geometry;
using MapBuilder.Shared;
using Microsoft.AspNetCore.Mvc;

namespace MapBuilder.Api.Controllers;

[ApiController]
[Route("{controller}")]
public class CellsController : ControllerBase, ICellsController
{
    [HttpGet("{action}/{level}/{latitudelo}/{longitudelo}/{latitudehi}/{longitudehi}")]
    public async Task<List<S2CellId>> GetCells(int level, double latitudelo, double longitudelo, double latitudehi, double longitudehi)
    {
        S2RegionCoverer cov = new S2RegionCoverer();
        cov.MaxLevel = level; // Set the maximum level
        cov.MinLevel = level; // Set the minimum level
        ///getcells/15/37.7749/-122.4194/37.7858/-122.3864
        S2LatLngRect regionRect = new S2LatLngRect(
            new S2LatLng(S1Angle.FromDegrees(latitudelo), S1Angle.FromDegrees(longitudelo)), // Bottom-left corner
            new S2LatLng(S1Angle.FromDegrees(latitudehi), S1Angle.FromDegrees(longitudehi)) // Top-right corner
        );

        List<S2CellId> cells = new List<S2CellId>();

        cells.AddRange(cov.GetCovering(regionRect));

        return cells;
    }


}