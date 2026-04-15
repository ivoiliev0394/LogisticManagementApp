using LogisticManagementApp.Domain.Assets.Air;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Air
{
    public static class AirCrewAssignmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.AirCrewAssignments.AnyAsync())
                return;

            var flights = await db.Flights.Take(15).ToListAsync();
            var crew = await db.AirCrewMembers.Take(15).ToListAsync();

            var list = new List<AirCrewAssignment>();

            for (int i = 0; i < 15 && i < flights.Count && i < crew.Count; i++)
            {
                list.Add(new AirCrewAssignment
                {
                    Id = Guid.NewGuid(),
                    FlightId = flights[i].Id,
                    AirCrewMemberId = crew[i].Id,
                    AssignedRole = crew[i].CrewRole,
                    AssignedAtUtc = DateTime.UtcNow.AddDays(-1),
                    Notes = "Assigned to flight",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.AirCrewAssignments.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}
