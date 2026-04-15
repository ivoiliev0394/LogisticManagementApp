using LogisticManagementApp.Domain.Operations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations
{
    public static class BookingLegSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.BookingLegs.AnyAsync())
                return;

            var bookings = await db.Bookings
                .OrderBy(b => b.RequestedAtUtc)
                .Take(15)
                .ToListAsync();

            var locations = await db.Locations
                .OrderBy(l => l.Name)
                .Take(15)
                .ToListAsync();

            if (!bookings.Any() || locations.Count < 2)
                return;

            var legs = new List<BookingLeg>();

            for (int i = 0; i < bookings.Count && i < locations.Count; i++)
            {
                var etd = (bookings[i].RequestedAtUtc ?? DateTime.UtcNow).AddDays(1);
                var eta = etd.AddDays(2);

                legs.Add(new BookingLeg
                {
                    Id = Guid.NewGuid(),
                    BookingId = bookings[i].Id,
                    LegNo = 1,
                    OriginLocationId = locations[i].Id,
                    DestinationLocationId = locations[(i + 1) % locations.Count].Id,
                    ETD_Utc = etd,
                    ETA_Utc = eta,
                    CarrierReference = $"LEG-REF-{8000 + i}",
                    Notes = "Primary booking leg",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.BookingLegs.AddRangeAsync(legs);
            await db.SaveChangesAsync();
        }
    }
}