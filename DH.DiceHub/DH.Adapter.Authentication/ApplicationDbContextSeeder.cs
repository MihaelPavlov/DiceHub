using DH.Adapter.Authentication.Entities;
using DH.Domain.Adapters.Authentication.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DH.Adapter.Authentication;

/// <summary>
/// Provides methods to seed the database with initial users and roles for the application.
/// </summary>
public static class ApplicationDbContextSeeder
{
    /// <summary>
    /// Seeds the initial users and roles into the ASP.NET Core Identity system.
    /// </summary>
    /// <param name="serviceProvider">The service provider to resolve dependencies.</param>
    public static async Task SeedUsers(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        #region Seed Roles

        var roles = Enum
            .GetValues(typeof(UserRole))
            .Cast<UserRole>()
            .ToDictionary(role => role.ToString(), role => (int)role);

        foreach (var role in roles)
            await EnsureRoleAsync(roleManager, role);

        #endregion Seed Roles

        #region Seed Users

        var superAdminUser = new ApplicationUser
        {
            UserName = "sa@dicehub.com",
            Email = "sa@dicehub.com",
            EmailConfirmed = true
        };

        await EnsureUserAsync(userManager, superAdminUser, "1qaz!QAZ", UserRole.SuperAdmin.ToString());

        #endregion Seed Users
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, KeyValuePair<string, int> role)
    {
        if (!await roleManager.RoleExistsAsync(role.Key))
            await roleManager.CreateAsync(new IdentityRole(role.Key) { Id = role.Value.ToString() });
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, ApplicationUser user, string password, string role)
    {
        var existingUser = await userManager.FindByEmailAsync(user.Email ?? string.Empty);
        if (existingUser == null)
        {
            var createUserResult = await userManager.CreateAsync(user, password);
            if (createUserResult.Succeeded)
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
