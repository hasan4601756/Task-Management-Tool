using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManagementTool.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAndAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            ApplicationRole[] roles = {
                new ApplicationRole
                {
                    Name = "User",
                    Description = "Regular User",
                    IsSystemRole = true
                },
                new ApplicationRole
                {
                    Name = "Admin",
                    Description = "Administration User",
                    IsSystemRole = true
                }
            };

            foreach (var role in roles)
            {
                if (role.Name != null && !await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(role);
                }
            }

            var adminEmail = "admin@tasktool.com";
            var adminPassword = "Admin@123";
            var adminFullName = "Administrator";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = adminFullName
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}