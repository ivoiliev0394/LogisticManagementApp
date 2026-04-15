using LogisticManagementApp.Domain.Companies;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Companies
{
    public static class CompanyBranchSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.CompanyBranches.AnyAsync())
                return;

            var companies = await db.Companies.OrderBy(x => x.Name).Take(15).ToListAsync();
            var addresses = await db.Addresses.OrderBy(x => x.City).Take(15).ToListAsync();

            var branches = new List<CompanyBranch>();

            for (int i = 0; i < 15; i++)
            {
                branches.Add(new CompanyBranch
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    AddressId = addresses[i].Id,
                    Name = $"{companies[i].Name} Branch",
                    BranchCode = $"BR-{i + 1:000}",
                    Phone = $"+35970020{i + 1:00}",
                    Email = $"branch{i + 1}@company.local",
                    IsHeadOffice = i < 5,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.CompanyBranches.AddRangeAsync(branches);
            await db.SaveChangesAsync();
        }
    }
}
