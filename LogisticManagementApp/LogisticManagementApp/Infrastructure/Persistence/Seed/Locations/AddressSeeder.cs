using LogisticManagementApp.Domain.Locations;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Locations
{
    public static class AddressSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Addresses.AnyAsync())
                return;

            var addresses = new List<Address>
        {
            new() { Id = Guid.NewGuid(), Country = "Bulgaria", City = "Sofia", PostalCode = "1000", Street = "Tsarigradsko Shose Blvd 47", Latitude = 42.6977m, Longitude = 23.3219m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Bulgaria", City = "Plovdiv", PostalCode = "4000", Street = "Brezovsko Shose 125", Latitude = 42.1354m, Longitude = 24.7453m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Bulgaria", City = "Varna", PostalCode = "9000", Street = "Devnya Street 12", Latitude = 43.2141m, Longitude = 27.9147m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Bulgaria", City = "Burgas", PostalCode = "8000", Street = "Transportna Street 8", Latitude = 42.5048m, Longitude = 27.4626m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Bulgaria", City = "Ruse", PostalCode = "7000", Street = "Tutrakan Blvd 18", Latitude = 43.8356m, Longitude = 25.9657m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Romania", City = "Bucharest", PostalCode = "010101", Street = "Calea Victoriei 101", Latitude = 44.4268m, Longitude = 26.1025m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Greece", City = "Thessaloniki", PostalCode = "54624", Street = "Egnatia 56", Latitude = 40.6401m, Longitude = 22.9444m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Turkey", City = "Istanbul", PostalCode = "34000", Street = "Kennedy Cad. 220", Latitude = 41.0082m, Longitude = 28.9784m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Serbia", City = "Belgrade", PostalCode = "11000", Street = "Bulevar Vojvode Misica 14", Latitude = 44.7866m, Longitude = 20.4489m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Germany", City = "Hamburg", PostalCode = "20457", Street = "Am Sandtorkai 30", Latitude = 53.5511m, Longitude = 9.9937m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Netherlands", City = "Rotterdam", PostalCode = "3011AA", Street = "Wijnhaven 70", Latitude = 51.9244m, Longitude = 4.4777m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "France", City = "Marseille", PostalCode = "13002", Street = "Quai du Port 20", Latitude = 43.2965m, Longitude = 5.3698m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Italy", City = "Genoa", PostalCode = "16121", Street = "Via Gramsci 44", Latitude = 44.4056m, Longitude = 8.9463m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Austria", City = "Vienna", PostalCode = "1010", Street = "Opernring 5", Latitude = 48.2082m, Longitude = 16.3738m, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Country = "Hungary", City = "Budapest", PostalCode = "1051", Street = "Széchenyi István tér 7", Latitude = 47.4979m, Longitude = 19.0402m, CreatedAtUtc = DateTime.UtcNow }
        };

            await db.Addresses.AddRangeAsync(addresses);
            await db.SaveChangesAsync();
        }
    }
}
