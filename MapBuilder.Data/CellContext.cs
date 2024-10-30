using Microsoft.EntityFrameworkCore;
using MapBuilder.Shared;

namespace MapBuilder.Data;

public class CellContext : DbContext
{
    public DbSet<Cell> Cells { get; set; }
    
    public string DbPath { get; }

    public CellContext()
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