using LogisticManagementApp.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Documents
{
    public static class FileResourceSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.FileResources.AnyAsync())
                return;

            var files = new List<FileResource>();

            for (int i = 1; i <= 15; i++)
            {
                files.Add(new FileResource
                {
                    Id = Guid.NewGuid(),
                    StorageKey = $"documents/2026/03/file-{i:000}.pdf",
                    FileName = $"document-{i:000}.pdf",
                    ContentType = "application/pdf",
                    SizeBytes = 150_000 + (i * 5_000),
                    Checksum = Guid.NewGuid().ToString("N"),
                    UploadedAtUtc = DateTime.UtcNow.AddDays(-i),
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.FileResources.AddRangeAsync(files);
            await db.SaveChangesAsync();
        }
    }
}
