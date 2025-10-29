using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cosmic3.data.models;

[Table("Inventory")]
public class Inventory : DbContext
{
    public int InventoryId { get; set; }
    public long Balance { get; set; }
    public ICollection<Item> Items { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public static async Task<Inventory?> Read(int inventoryId)
    {
        await using var db = new CosmicDbContext();
        
        var inventory = from u in db.Inventories
            where u.InventoryId == inventoryId
            select u;

        return await inventory.FirstOrDefaultAsync();
    }

    public static async Task Update(Inventory inventory)
    {
        await using var db = new CosmicDbContext();
        db.Inventories.Update(inventory); 
        
        await db.SaveChangesAsync();
    }

    public static async Task Delete(Inventory inventory)
    {
        await using var db = new CosmicDbContext();
        db.Inventories.Remove(inventory);
        
        await db.SaveChangesAsync();
    }

    public static async Task<Inventory?> Pick(User user)
    {
        Inventory? inventory = null;
        inventory = await Read(user.Inventory.InventoryId);
        inventory ??= new Inventory()
        {
            Balance = 0,
            Items = new List<Item>(),
            UserId = user.UserId
        };

        return inventory;
    }
}