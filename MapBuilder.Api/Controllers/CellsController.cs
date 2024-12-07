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
        if (level < 0 || level > 30)
        {
            throw new ArgumentOutOfRangeException(nameof(level), "Level must be between 0 and 30");
        }
        
        S2RegionCoverer cov = new S2RegionCoverer
        {
            MaxLevel = level,
            MinLevel = level
        };

        S2LatLngRect regionRect = new S2LatLngRect(
            new S2LatLng(S1Angle.FromDegrees(latitudelo), S1Angle.FromDegrees(longitudelo)),
            new S2LatLng(S1Angle.FromDegrees(latitudehi), S1Angle.FromDegrees(longitudehi))
        );

        List<S2CellId> cells = new List<S2CellId>();
        // Get the covering cells
        S2RegionCoverer.GetSimpleCovering(regionRect, regionRect.Center.ToPoint(),level, cells);

        // Filter cells to ensure they are at the specified level
        //cells.AddRange(coveringCells);
        //cells.AddRange(coveringCells.Where(cell => cell.Level == level));

        // Log the levels of the returned cells
        foreach (var cell in cells)
        {
            Console.WriteLine($"Cell ID: {cell}, Level: {cell.Level}");
        }

        return cells;
    }
}