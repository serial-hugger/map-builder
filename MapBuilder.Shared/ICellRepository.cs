namespace MapBuilder.Shared;

public interface ICellRepository
{
    Task AddCell(Cell cell);
    Task UpdateCell(Cell cell);
    Task<Cell?> GetCellByTokenAsync(string token);
}