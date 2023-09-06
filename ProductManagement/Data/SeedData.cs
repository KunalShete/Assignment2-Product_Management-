using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using ProductManagement.Data;

namespace ProductManagement.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            await InitializeRoles(roleManager);
            await InitializeUsers(userManager, roleManager);
        }


        private static async Task InitializeUsers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Create SuperAdmin user
            var superAdmin = await userManager.FindByEmailAsync("superadmin@example.com");
            if (superAdmin == null)
            {
                superAdmin = new IdentityUser
                {
                    UserName = "superadmin@example.com",
                    Email = "superadmin@example.com"
                };
                await userManager.CreateAsync(superAdmin, "Password123!");
            }

            var superAdminRole = await roleManager.FindByNameAsync("SuperAdmin");
            if (superAdminRole != null)
            {
                await userManager.AddToRoleAsync(superAdmin, superAdminRole.Name);
            }

            // Create Admin user and add Admin role
            var admin = await userManager.FindByEmailAsync("admin@example.com");
            if (admin == null)
            {
                admin = new IdentityUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com"
                };
                await userManager.CreateAsync(admin, "Password123!");
            }

            var adminRole = await roleManager.FindByNameAsync("Admin");
            if (adminRole != null)
            {
                await userManager.AddToRoleAsync(admin, adminRole.Name);
            }

            // Create User user and add User role
            var user = await userManager.FindByEmailAsync("user@example.com");
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = "user@example.com",
                    Email = "user@example.com"
                };
                await userManager.CreateAsync(user, "Password123!");
            }

            var userRole = await roleManager.FindByNameAsync("User");
            if (userRole != null)
            {
                await userManager.AddToRoleAsync(user, userRole.Name);
            }
        }

        private static async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

    }
}
