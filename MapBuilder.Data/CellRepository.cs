using MapBuilder.Shared;
using Microsoft.EntityFrameworkCore;

namespace MapBuilder.Data;

public class CellRepository : ICellRepository
{
    private readonly CellContext _context = new();

    public async Task AddCell(Cell cell)
    {
        await _context.Cells.AddAsync(cell);
        //await _context.SaveChangesAsync();
    }
    public async Task UpdateCell(Cell cell)
    {
        _context.Entry(cell).State = EntityState.Modified;
        //await _context.SaveChangesAsync();
    }

    public async Task<List<Cell>> GetAllCells()
    {
        return await _context.Cells.ToListAsync();
    }
    public async Task<Cell?> GetCellByTokenAsync(string token)
    {
        try
        {
            return await _context.Cells.Where(o => o.CellToken == token)
                .Include(c => c.Nodes)
                .Include(c => c.Ways)
                .FirstAsync();
        }
        catch
        {
            return null;
        }
    }
}