using MapBuilder.Controllers;

namespace MapBuilder;

public class MapBuilder
{
    private readonly CellsController _cellsController;
    private readonly OSMController _osmController;
    
    public List<Cell> cells { get; private set; } = new List<Cell>();
    
    public MapBuilder(CellsController cellsController, OSMController osmController)
    {
        _cellsController = cellsController;
        _osmController = osmController;
    }

    public void BuildMap(List<ulong> cellTokens)
    {
        
    }
}