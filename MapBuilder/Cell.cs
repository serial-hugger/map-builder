using Google.Common.Geometry;

namespace MapBuilder;

public class Cell
{
    public ulong cellToken;
    public string jsonData;
    public int level;

    public Cell(ulong cellToken, int level)
    {
        S2Cell cell = new S2Cell(new S2CellId(cellToken));
    }
}