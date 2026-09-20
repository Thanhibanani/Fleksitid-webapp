using Fleksitid.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fleksitid.Data
{
    /// <summary>Migrerer databasen og sørger for at standardrollene finnes.</summary>
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            await db.Database.MigrateAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
