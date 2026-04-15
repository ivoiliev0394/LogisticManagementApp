using LogisticManagementApp.Domain.Documents;
using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Enums.Shipments;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Infrastructure.Persistence.Seed.Documents
{
    public static class DocumentTemplateSeeder
    {
        public static async Task SeedAsync(LogisticAppDbContext db)
        {
            if (await db.DocumentTemplates.AnyAsync())
                return;

            var files = await db.FileResources.Take(15).ToListAsync();
            var companies = await db.Companies.Take(15).ToListAsync();

            var temTypes = Enum.GetValues<DocumentTemplateType>();
            int temType = 1;

            var templates = new List<DocumentTemplate>();

            for (int i = 0; i < 15 && i < files.Count && i < companies.Count; i++)
            {
                templates.Add(new DocumentTemplate
                {
                    Id = Guid.NewGuid(),
                    Name = $"Template {i + 1}",
                    TemplateType = (DocumentTemplateType)temType,
                    CompanyId = companies[i].Id,
                    FileResourceId = files[i].Id,
                    IsDefault = i < 3,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (temType < temTypes.Length) temType++;
                else temType = 1;
            }

            await db.DocumentTemplates.AddRangeAsync(templates);
            await db.SaveChangesAsync();
        }
    }
}
