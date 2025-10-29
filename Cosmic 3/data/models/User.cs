using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Cosmic3.data.models;

[Table("User")]
public class User
{
    public int UserId { get; set; }

    public string PlatformId { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    
    public Inventory? Inventory { get; set; }

    public static async Task Create(User user)
    {
        await using var db = new CosmicDbContext();
        await db.Users.AddAsync(user);
        
        await db.SaveChangesAsync();
    }

    public static async Task<User?> Read(string platformId)
    {
        await using var db = new CosmicDbContext();
        
        var user = from u in db.Users
            where u.PlatformId == platformId
                select u;

        return await user.FirstOrDefaultAsync();
    }

    public static async Task Update(User user)
    {
        await using var db = new CosmicDbContext();
        db.Users.Update(user);
        
        await db.SaveChangesAsync();
    }

    public static async Task Delete(User user)
    {
        await using var db = new CosmicDbContext();
        db.Users.Remove(user);
        
        await db.SaveChangesAsync();
    }
}