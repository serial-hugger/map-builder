namespace MapBuilder.Data;

public interface ICellRepository
{
    Task AddCell(CellModel cell);
    Task<CellModel> GetCellByTokenAsync(string token);
}