using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Planning
{
    public static class ResourceAvailabilitySeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ResourceAvailabilities.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var list = new List<ResourceAvailability>();

            var types = Enum.GetValues<ResourceType>();
            int type = 1;

            for (int i = 0; i < companies.Count; i++)
            {
                var from = DateTime.UtcNow.Date.AddDays(i).AddHours(8);
                var to = from.AddHours(8);

                list.Add(new ResourceAvailability
                {
                    Id = Guid.NewGuid(),
                    ResourceType = (ResourceType) type, // смени ако enum-ът ти няма Company
                    ResourceId = companies[i].Id,
                    AvailableFromUtc = from,
                    AvailableToUtc = to,
                    Status = i % 2 == 0
                        ? AvailabilityStatus.Available
                        : AvailabilityStatus.Unavailable,
                    Reason = i % 2 == 0 ? null : "Planned maintenance",
                    Notes = "Availability schedule",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (type < types.Length)
                    type++;
                else
                    type = 1;
            }

            await db.ResourceAvailabilities.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}