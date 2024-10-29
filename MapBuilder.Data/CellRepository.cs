using Microsoft.EntityFrameworkCore;

namespace MapBuilder.Data;

public class CellRepository : ICellRepository
{
    private readonly DbContext _context;
    public CellRepository()
    {
        _context = new DbContext();
    }
    public async Task AddCell(CellModel cell)
    {
        await _context.Cells.AddAsync(cell);
        await _context.SaveChangesAsync();
    }

    public async Task<CellModel> GetCellByTokenAsync(string token)
    {
        return await _context.Cells.Where(o => o.CellToken== token).FirstOrDefaultAsync();
    }
}