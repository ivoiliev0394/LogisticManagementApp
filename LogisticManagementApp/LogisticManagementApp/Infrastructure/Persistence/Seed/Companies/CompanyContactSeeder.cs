using LogisticManagementApp.Domain.Companies;
using Microsoft.EntityFrameworkCore;  

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Companies
{
    public static class CompanyContactSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.CompanyContacts.AnyAsync())
                return;

            var companies = await db.Companies.OrderBy(x => x.Name).Take(15).ToListAsync();

            var contacts = new List<CompanyContact>();

            for (int i = 0; i < companies.Count; i++)
            {
                contacts.Add(new CompanyContact
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companies[i].Id,
                    FullName = $"Primary Contact {i + 1}",
                    Email = $"contact{i + 1}@{companies[i].Name.Replace(" ", "").ToLower()}.com",
                    Phone = $"+359888100{i + 1:00}",
                    RoleTitle = "Operations Manager",
                    IsPrimary = true,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.CompanyContacts.AddRangeAsync(contacts);
            await db.SaveChangesAsync();
        }
    }
}
