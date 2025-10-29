using Cosmic3.data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cosmic3.data;

public class CosmicDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Item> Items { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("NPGSQL_CONNECTION_STRING");
        optionsBuilder.UseNpgsql(connectionString);
        base.OnConfiguring(optionsBuilder);
    }
    
    public static async void EnsureCreated()
    {
        await using var db = new CosmicDbContext();
        await db.Database.EnsureCreatedAsync();
    }
}