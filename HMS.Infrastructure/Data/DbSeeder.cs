using HMS.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace HMS.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles = { "Admin", "Manager", "Guest" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
                }
            }
        }

        public static async Task SeedAdmin(UserManager<ApplicationUser> userManager)
        {
            var email = "admin@hms.com";

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = "Admin",
                    LastName = "User",
                    PersonalNumber = "00000000000"
                };

                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}