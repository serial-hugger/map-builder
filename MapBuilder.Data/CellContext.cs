using Microsoft.EntityFrameworkCore;
using MapBuilder.Shared;

namespace MapBuilder.Data;

public class CellContext : DbContext
{
    public DbSet<Cell> Cells { get; set; }
    
    public string DbPath { get; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cell>()
            .HasKey(c => c.Id);
        modelBuilder.Entity<Cell>()
            .HasMany(c => c.Nodes)
            .WithOne()
            .HasForeignKey(n => n.CellId);
        modelBuilder.Entity<Cell>()
            .HasMany(c => c.Ways)
            .WithOne()
            .HasForeignKey(w => w.CellId);
        
        modelBuilder.Entity<Node>()
            .HasKey(n => n.Id);

        modelBuilder.Entity<Way>()
            .HasKey(w => w.Id);
    }

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