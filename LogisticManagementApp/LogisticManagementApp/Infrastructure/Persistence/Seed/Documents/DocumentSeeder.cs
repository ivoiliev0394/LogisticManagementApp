using LogisticManagementApp.Domain.Documents;
using LogisticManagementApp.Domain.Enums.Billing;
using LogisticManagementApp.Domain.Enums.Shipments;

using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Documents
{
    public static class DocumentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.Documents.AnyAsync())
                return;

            var files = await db.FileResources.Take(15).ToListAsync();
            var shipments = await db.Shipments.Take(15).ToListAsync();
            var companies = await db.Companies.Take(15).ToListAsync();

            var types = Enum.GetValues<DocumentType>(); 
            int type = 1;

            var docs = new List<Document>();

            for (int i = 0; i < 15 && i < shipments.Count && i < files.Count && i < companies.Count; i++)
            {
                docs.Add(new Document
                {
                    Id = Guid.NewGuid(),
                    ShipmentId = shipments[i].Id,
                    DocumentType = (DocumentType)type,
                    FileResourceId = files[i].Id,
                    DocumentNo = $"DOC-{9000 + i}",
                    IssuedAtUtc = DateTime.UtcNow.AddDays(-i),
                    IssuedByCompanyId = companies[i].Id,
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (type < types.Length) type++; 
                else type = 1;
            }

            await db.Documents.AddRangeAsync(docs);
            await db.SaveChangesAsync();
        }
    }
}
