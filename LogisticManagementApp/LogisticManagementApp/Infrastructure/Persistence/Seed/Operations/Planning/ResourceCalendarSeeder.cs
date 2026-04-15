using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations.Planning
{
    public static class ResourceCalendarSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.ResourceCalendars.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var list = new List<ResourceCalendar>();

            var types = Enum.GetValues<ResourceType>();
            int type = 1;

            for (int i = 0; i < companies.Count; i++)
            {
                list.Add(new ResourceCalendar
                {
                    Id = Guid.NewGuid(),
                    ResourceType = (ResourceType)type, // или друго валидно enum value
                    ResourceId = companies[i].Id,
                    DateUtc = DateTime.UtcNow.Date.AddDays(i),
                    Status = AvailabilityStatus.Available,
                    PlannedCapacity = 1000m,
                    ReservedCapacity = 400m,
                    UsedCapacity = 300m,
                    Notes = "Daily resource plan",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (type < types.Length)
                    type++;
                else
                    type = 1;
            }

            await db.ResourceCalendars.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}