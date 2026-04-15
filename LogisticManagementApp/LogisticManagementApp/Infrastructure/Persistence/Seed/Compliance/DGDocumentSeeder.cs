using LogisticManagementApp.Domain.Compliance;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Compliance
{
    public static class DGDocumentSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.DGDocuments.AnyAsync())
                return;

            var declarations = await db.DangerousGoodsDeclarations.Take(15).ToListAsync();
            var files = await db.FileResources.Take(15).ToListAsync();

            if (!declarations.Any() || !files.Any())
                return;

            var count = Math.Min(declarations.Count, files.Count);
            var list = new List<DGDocument>();

            for (int i = 0; i < count; i++)
            {
                list.Add(new DGDocument
                {
                    Id = Guid.NewGuid(),
                    DangerousGoodsDeclarationId = declarations[i].Id,
                    FileResourceId = files[i].Id,
                    DocumentName = "DG Form",
                    Notes = "Attached DG document",
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            await db.DGDocuments.AddRangeAsync(list);
            await db.SaveChangesAsync();
        }
    }
}