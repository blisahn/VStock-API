using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VBorsa_API.Core.Entities.Identity;
using VBorsa_API.Core.Entities.Identity.Helpers;

namespace VBorsa_API.Persistence.Seed;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        async Task EnsureRoleAsync(string role, IEnumerable<string> permissions)
        {
            var r = await roleMgr.FindByNameAsync(role);
            if (r == null)
            {
                r = new AppRole { Name = role };
                await roleMgr.CreateAsync(r);
            }

            var desired = permissions?.ToHashSet() ?? new HashSet<string>();
            var existing = await roleMgr.GetClaimsAsync(r);

            // 1) Fazla permission claim'lerini sil
            foreach (var c in existing.Where(c => c.Type == "permission" && !desired.Contains(c.Value)).ToList())
                await roleMgr.RemoveClaimAsync(r, c);

            // 2) Eksik olanları ekle
            foreach (var p in desired)
                if (!existing.Any(c => c.Type == "permission" && c.Value == p))
                    await roleMgr.AddClaimAsync(r, new Claim("permission", p));
        }

        await EnsureRoleAsync("User", new[]
        {
            Permissions.ViewProfile,
            Permissions.EditProfile
        });

        await EnsureRoleAsync("Moderator", new[]
        {
            Permissions.UsersView,
            Permissions.ViewProfile,
            Permissions.EditProfile
        });

        await EnsureRoleAsync("Admin", Permissions.All);

        var adminEmail = "admin@admin.com";
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new AppUser { Id = Guid.NewGuid(), UserName = "admin", FullName = "admin", Email = adminEmail };
            await userMgr.CreateAsync(admin, "admin");
            await userMgr.AddToRolesAsync(admin, new[] { "Admin" });
        }

        var moderatorEmail = "mod@mod.com";
        var moderator = await userMgr.FindByEmailAsync(moderatorEmail);
        if (moderator == null)
        {
            moderator = new AppUser { Id = Guid.NewGuid(), UserName = "mod", FullName = "mod", Email = moderatorEmail };
            await userMgr.CreateAsync(moderator, "mod");
            await userMgr.AddToRolesAsync(moderator, new[] { "Moderator" });
        }

        var userEmail = "amos@amos.com";
        var user = await userMgr.FindByEmailAsync(userEmail);
        if (user == null)
        {
            user = new AppUser { Id = Guid.NewGuid(), UserName = "amos", FullName = "amos", Email = userEmail };
            await userMgr.CreateAsync(user, "amos");
            await userMgr.AddToRolesAsync(user, new[] { "User" });
        }
    }
}
// MODERATOR
//  "permission": [
// "Users.View",
//
// "Settings.View",
//
// "Users.Manage",
//
// "Profile.View",
//
// "Profile.Edit"
//
//     ],

// USER
// "permission": [
//
// "Profile.View",
//
// "Profile.Edit"
//
//     ],

// ADMIN
// "permission": [
//
// "Profile.View",
//
// "Profile.Edit",
//
// "Users.View",
//
// "Users.Manage",
//
// "Settings.View",
//
// "Settings.Manage"
//
//     ],