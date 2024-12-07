using MapBuilder.Shared;
using Microsoft.EntityFrameworkCore;

namespace MapBuilder.Data;

public class CellRepository : ICellRepository, IDisposable, IAsyncDisposable
{
    private readonly CellContext _context = new();

    public async Task AddCell(Cell cell)
    {
        await _context.Cells.AddAsync(cell);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateCell(Cell cell)
    {
        _context.Entry(cell).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Cell>> GetAllCells()
    {
        return await _context.Cells.ToListAsync();
    }

    public async Task<int> GetHighestGenerationVersion()
    {
        try
        {
            return await _context.Cells.MaxAsync(o => o.GenerationVersion);
        }
        catch
        {
            return 0;
        }
    }
    public async Task<Cell?> GetCellByTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null; // Early exit for invalid token
        }
        try
        {
            return await _context.Cells.Where(o => o.CellToken == token)
                .Include(c => c.Nodes)
                .Include(c => c.Ways)
                .FirstOrDefaultAsync();
        }
        catch
        {
            return null;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}