using System;
using System.Threading.Tasks;
using BizNews.Models;
using Microsoft.AspNetCore.Identity;

public class DataSeeder
{
    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("User"))
        {
            var userRole = new IdentityRole("User");
            await roleManager.CreateAsync(userRole);
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var adminRole = new IdentityRole("Admin");
            await roleManager.CreateAsync(adminRole);
        }

        if (!await roleManager.RoleExistsAsync("Editor"))
        {
            var editorRole = new IdentityRole("Editor");
            await roleManager.CreateAsync(editorRole);
        }

        if (!await roleManager.RoleExistsAsync("Author"))
        {
            var authorRole = new IdentityRole("Author");
            await roleManager.CreateAsync(authorRole);
        }
    }

    public static async Task SeedUsers(UserManager<User> userManager)
    {
        var existingUser = await userManager.FindByEmailAsync("babayefff02@gmail.com");

        if (existingUser == null)
        {
            var user = new User
            {
                UserName = "elmar02",
                Email = "babayefff02@gmail.com"
            };

            var result = await userManager.CreateAsync(user, "Elmar123");

            if (result.Succeeded)
            {
                // Assign all roles to the user
                await userManager.AddToRolesAsync(user, new[] { "User", "Admin", "Editor", "Author" });
            }
        }
    }
}
