using HMS.Core.Entities;
using Microsoft.AspNetCore.Identity;
namespace HMS.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Manager", "Guest" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }


        public static async Task SeedAdmin( UserManager<ApplicationUser> userManager)
        {
            var email = "admin@test.com";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = "System",
                    LastName = "Admin",
                    PersonalNumber = "00000000000"
                };

                await userManager.CreateAsync(admin, "Password123.");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}