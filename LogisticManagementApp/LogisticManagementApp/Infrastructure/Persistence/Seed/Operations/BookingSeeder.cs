using LogisticManagementApp.Domain.Enums.Assets;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Operations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Operations
{
    public static class BookingSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Bookings.AnyAsync())
                return;

            var companies = await db.Companies
                .OrderBy(c => c.Name)
                .Take(15)
                .ToListAsync();

            if (!companies.Any())
                return;

            var bookings = new List<Booking>();
            
            var types = Enum.GetValues<TransportMode>();
            int type = 1;

            for (int i = 0; i < companies.Count; i++)
            {
                bookings.Add(new Booking
                {
                    Id = Guid.NewGuid(),
                    BookingNo = $"BKG-{5000 + i}",
                    CarrierCompanyId = companies[i].Id,
                    TransportMode = (TransportMode)type,
                    RequestedAtUtc = DateTime.UtcNow.AddDays(-i),
                    CarrierReference = $"REF-{7000 + i}",
                    Notes = "Customer booking request",
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (type < types.Length)
                    type++;
                else
                    type = 1;
            }

            await db.Bookings.AddRangeAsync(bookings);
            await db.SaveChangesAsync();
        }
    }
}