using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LogisticManagementApp.Domain.Enums.Companies;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            string[] roles =
            {
                RoleNames.Admin,
                RoleNames.Company,
                RoleNames.Client
            };

            foreach (var roleName in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                var result = await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = roleName
                });

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(
                        $"Failed to create role '{roleName}': " +
                        string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
