using LogisticManagementApp.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Documents
{
    public static class DocumentVersionSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.DocumentVersions.AnyAsync())
                return;

            var documents = await db.Documents.Take(15).ToListAsync();
            var files = await db.FileResources.OrderBy(x => x.UploadedAtUtc).Take(15).ToListAsync();

            var versions = new List<DocumentVersion>();

            for (int i = 0; i < 15 && i < documents.Count && i < files.Count; i++)
            {
                versions.Add(new DocumentVersion
                {
                    Id = Guid.NewGuid(),
                    DocumentId = documents[i].Id,
                    FileResourceId = files[i].Id,
                    VersionNo = 1,
                    CreatedOnUtc = DateTime.UtcNow.AddDays(-i),
                    ChangeDescription = "Initial version",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.DocumentVersions.AddRangeAsync(versions);
            await db.SaveChangesAsync();
        }
    }
}
