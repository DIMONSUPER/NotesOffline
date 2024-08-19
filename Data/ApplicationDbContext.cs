using Microsoft.EntityFrameworkCore;
using NotesOffline.Models;

namespace NotesOffline.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Note> Notes { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
        SQLitePCL.Batteries_V2.Init();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Constants.DB_NAME);

        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }
}
