using LogisticManagementApp.Domain.Clients;
using LogisticManagementApp.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Identity;

public static class UserSeeder
{
    public static async Task SeedAsync(
        LogisticAppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        await SeedAdministratorAsync(userManager);
        await SeedCompanyUsersAsync(db, userManager);
    }

    private static async Task SeedAdministratorAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminUsername = "admin";
        const string adminEmail = "admin@logisticapp.com";
        const string adminPassword = "Admin123!";

        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminUsername,
                Email = adminEmail,
                EmailConfirmed = true,
                LockoutEnabled = true
            };

            var createResult = await userManager.CreateAsync(admin, adminPassword);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to create administrator user: " +
                    string.Join("; ", createResult.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(admin, RoleNames.Admin))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(admin, RoleNames.Admin);

            if (!addToRoleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Failed to assign Administrator role: " +
                    string.Join("; ", addToRoleResult.Errors.Select(e => e.Description)));
            }
        }
    }

    private static async Task SeedCompanyUsersAsync(
        LogisticAppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        var companies = await db.Companies
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        foreach (var company in companies)
        {
            var normalizedName = Normalize(company.Name);
            var email = $"{normalizedName}@company.logisticapp.com";
            var username = $"company_{normalizedName}";
            string password = $"Company123!";

            var existingUser = await userManager.Users
                .FirstOrDefaultAsync(u => u.CompanyId == company.Id);

            if (existingUser == null)
            {
                existingUser = await userManager.FindByEmailAsync(email);
            }

            if (existingUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    CompanyId = company.Id
                };

                var createResult = await userManager.CreateAsync(user, password);

                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create company user for '{company.Name}': " +
                        string.Join("; ", createResult.Errors.Select(e => e.Description)));
                }

                existingUser = user;
            }
            else if (existingUser.CompanyId != company.Id)
            {
                existingUser.CompanyId = company.Id;

                var updateResult = await userManager.UpdateAsync(existingUser);

                if (!updateResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to update company user for '{company.Name}': " +
                        string.Join("; ", updateResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(existingUser, RoleNames.Company))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(existingUser, RoleNames.Company);

                if (!addToRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to assign Company role for '{company.Name}': " +
                        string.Join("; ", addToRoleResult.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    private static string Normalize(string value)
    {
        return new string(value
            .ToLowerInvariant()
            .Where(char.IsLetterOrDigit)
            .ToArray());
    }
}