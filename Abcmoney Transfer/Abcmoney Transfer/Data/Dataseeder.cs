using Abcmoney_Transfer;
using Abcmoney_Transfer.Data;
using Abcmoney_Transfer.Models;
using AbcmoneyTransfer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Abcmoney_Transfer.Models.IdentityModel;
public class DataSeeder
{
    public async Task SeedSuperAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        //var seedStatus = await dbContext.Set<Seedstatus>().FirstOrDefaultAsync();
        //if (seedStatus?.IsSeeded == true)
        //    return;

        const string superAdminRoleName = "SuperAdmin";
        const string superAdminEmail = "superadmin@example.com";
        const string superAdminPassword = "SuperSecurePassword123!";

        // 1. Ensure Role exists
        //var superAdminRole = await roleManager.FindByNameAsync(superAdminRoleName);
        //if (superAdminRole == null)
        //{
        var role = new IdentityRole(superAdminRoleName);
        var roleResult = await roleManager.CreateAsync(role);

        if (!roleResult.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to create role '{superAdminRoleName}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}"
                );
        //}

        // 2. Ensure User exists
        //var superAdminUser = await userManager.FindByEmailAsync(superAdminEmail);
        //if (superAdminUser == null)
        //{
           var superAdminUser = new AppUser
            {
                UserName = superAdminEmail,
                Email = superAdminEmail,
                EmailConfirmed = true
            };

            var userResult = await userManager.CreateAsync(superAdminUser, superAdminPassword);
            if (!userResult.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to create SuperAdmin user: {string.Join(", ", userResult.Errors.Select(e => e.Description))}"
                );
        //}

        // 3. Ensure User has Role
        if (!await userManager.IsInRoleAsync(superAdminUser, superAdminRoleName))
        {
            var roleAssignResult = await userManager.AddToRoleAsync(superAdminUser, superAdminRoleName);
            if (!roleAssignResult.Succeeded)
                throw new InvalidOperationException(
                    $"Failed to assign role '{superAdminRoleName}' to SuperAdmin user: {string.Join(", ", roleAssignResult.Errors.Select(e => e.Description))}"
                );
        }

        // 4. Update Seed Status
        //if (seedStatus == null)
        //{
        //    dbContext.Set<Seedstatus>().Add(new Seedstatus
        //    {
        //        IsSeeded = true,
        //        LastSeededOn = DateTime.UtcNow
        //    });
        //}
        //else
        //{
        //    seedStatus.IsSeeded = true;
        //    seedStatus.LastSeededOn = DateTime.UtcNow;
        //}

        await dbContext.SaveChangesAsync();
    }
}


