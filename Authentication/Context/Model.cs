using Authentication;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

public class UserContext : DbContext
{
    public DbSet<User> UserSet { get; set; }

    public string DbPath { get; }

    public UserContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "user.db");

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Username);
    }


}

