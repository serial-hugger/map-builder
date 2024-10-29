using Microsoft.EntityFrameworkCore;

namespace MapBuilder.Data;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<CellModel> Cells { get; set; }
    
    public string DbPath { get; }

    public DbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "cells.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
public class CellModel()
{
    public int Id { get; set; }
    public string CellToken { get; set; }
    public List<NodeModel> Nodes { get; set; } = new();
}

public class NodeModel()
{
    public int Id { get; set; }
    public Int64 NodeId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Int64? WayId { get; set; }
}