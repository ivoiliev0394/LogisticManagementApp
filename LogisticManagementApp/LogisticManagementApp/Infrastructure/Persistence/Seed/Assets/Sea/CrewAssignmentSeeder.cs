using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Assets.Sea
{
    public static class CrewAssignmentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.CrewAssignments.AnyAsync())
                return;

            var voyages = await db.Voyages.Take(15).ToListAsync();
            var crewMembers = await db.VesselCrewMembers.Take(15).ToListAsync();

            var assignments = new List<CrewAssignment>();

            for (int i = 0; i < 15 && i < voyages.Count && i < crewMembers.Count; i++)
            {
                assignments.Add(new CrewAssignment
                {
                    Id = Guid.NewGuid(),
                    VoyageId = voyages[i].Id,
                    VesselCrewMemberId = crewMembers[i].Id,
                    AssignedRole = crewMembers[i].CrewRole,
                    AssignedAtUtc = DateTime.UtcNow.AddDays(-10 + i),
                    FromUtc = DateTime.UtcNow.AddDays(-10 + i),
                    ToUtc = DateTime.UtcNow.AddDays(10 + i),
                    Notes = "Assigned for current voyage",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.CrewAssignments.AddRangeAsync(assignments);
            await db.SaveChangesAsync();
        }
    }
}
