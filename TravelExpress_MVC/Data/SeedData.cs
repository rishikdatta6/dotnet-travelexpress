using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "Admin", "User" };

        // ✅ Step 1: Create roles if they don't exist
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // ✅ Step 2: Create Admin user if not already exists
        var adminUser = await userManager.FindByEmailAsync("admin@travelexpress.com");
        if (adminUser == null)
        {
            var newAdmin = new ApplicationUser
            {
                UserName = "admin@travelexpress.com",
                Email = "admin@travelexpress.com",
                EmailConfirmed = true,
                Name = "Admin User"
            };

            string adminPassword = "Admin@123";  // 🔐 Use a strong password in production

            var createAdmin = await userManager.CreateAsync(newAdmin, adminPassword);
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin"); // 🔒 Only Admin role
            }
        }

        // ✅ (Optional) Step 3: Create default User (for testing)
        var regularUser = await userManager.FindByEmailAsync("user@travelexpress.com");
        if (regularUser == null)
        {
            var newUser = new ApplicationUser
            {
                UserName = "user@travelexpress.com",
                Email = "user@travelexpress.com",
                EmailConfirmed = true,
                Name = "Demo User"
            };

            string userPassword = "User@123";

            var createUser = await userManager.CreateAsync(newUser, userPassword);
            if (createUser.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, "User");
            }
        }
    }
}
