using MapBuilder.Shared;
using Microsoft.EntityFrameworkCore;

namespace MapBuilder.Data;

public class CellRepository : ICellRepository
{
    private readonly CellContext _context;
    public CellRepository()
    {
        _context = new CellContext();
    }
    public async Task AddCell(Cell cell)
    {
        await _context.Cells.AddAsync(cell);
        await _context.SaveChangesAsync();
    }
    public async Task<Cell> GetCellByTokenAsync(string token)
    {
        try
        {
            return await _context.Cells.Where(o => o.CellToken == token).FirstOrDefaultAsync();
        }
        catch
        {
            return null;
        }
    }
}