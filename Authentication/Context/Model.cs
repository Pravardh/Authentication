using Authentication.Models;
using Microsoft.EntityFrameworkCore;

public class UserContext : DbContext
{
    public DbSet<User> UserSet { get; set; }

    public DbSet<PlayerStat> PlayerStats { get; set; }

    public string DbPath { get; }

    public UserContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "user.db");

        Database.EnsureCreated();

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Username);

        modelBuilder.Entity<PlayerStat>()
            .HasOne(s => s.User)
            .WithMany(u => u.PlayerStats)
            .HasForeignKey(s => s.Username);
    }


}

