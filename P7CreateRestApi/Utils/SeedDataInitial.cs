using Dot.Net.WebApi.Domain;
using Microsoft.AspNetCore.Identity;

namespace P7CreateRestApi.Utils
{
    public class SeedDataInitial
    {
        public static async Task SeedData(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();

            foreach (var role in Models.RoleCollection.Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminConfig = configuration.GetSection("AdminUser");
            var adminEmail = adminConfig["Email"];
            var adminPassword = adminConfig["Password"];
            var adminUserName = adminConfig["UserName"];
            var adminFullname = adminConfig["Fullname"];

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new User
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    Fullname = adminFullname,
                };

                var result = await userManager.CreateAsync(newAdmin, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}
