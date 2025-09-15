using ABCExchange.Models;
using Abcmoney_Transfer.Models;
using Microsoft.AspNetCore.Identity;

public class DataSeeder
{
    public async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Identity>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<Userlogin>>();

        // Check if already seeded
        var seedStatus = await dbContext.Set<SeedStatus>().FirstOrDefaultAsync();
        if (seedStatus?.IsSeeded == true)
            return;

        // Seed roles and SuperAdmin
        var superAdminRoleName = "SuperAdmin";
        var superAdminEmail = "superadmin@example.com";
        var superAdminPassword = "SuperSecurePassword123!";

        // Ensure the SuperAdmin role exists
        var superAdminRole = await roleManager.FindByNameAsync(superAdminRoleName);
        if (superAdminRole == null)
        {
            superAdminRole = new Identity{Name = superAdminRoleName }; // Assign role name directly
            var roleCreationResult = await roleManager.CreateAsync(superAdminRole);
            if (!roleCreationResult.Succeeded)
            {
                // Handle role creation failure (e.g., log or throw an exception)
                throw new InvalidOperationException($"Failed to create role: {superAdminRoleName}");
            }
        }
        // Ensure the SuperAdmin user exists
        var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
        if (superAdminUser == null)
        {
            superAdminUser = new Userlogin
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true
            };
            var userCreationResult = await userManager.CreateAsync(superAdminUser, superAdminPassword);
            if (!userCreationResult.Succeeded)
            {
                // Handle user creation failure (e.g., log or throw an exception)
                throw new InvalidOperationException("Failed to create SuperAdmin user.");
            }

            // Assign SuperAdmin role to the user
            var roleAssignmentResult = await userManager.AddToRoleAsync(superAdminUser, superAdminRoleName);
            if (!roleAssignmentResult.Succeeded)
            {
                // Handle role assignment failure
                throw new InvalidOperationException("Failed to assign SuperAdmin role to the user.");
            }
        }
        // Mark as seeded
        if (seedStatus == null) 
        {
            dbContext.Set<SeedStatus>().Add(new SeedStatus { IsSeeded = true, LastSeededOn = DateTime.UtcNow });
        }
        else
        {
            seedStatus.IsSeeded = true;
            seedStatus.LastSeededOn = DateTime.UtcNow;
        }
        await dbContext.SaveChangesAsync();
    }
}
 
