using LogisticManagementApp.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Documents
{
    public class DocumentTemplateConfiguration : IEntityTypeConfiguration<DocumentTemplate>
    {
        public void Configure(EntityTypeBuilder<DocumentTemplate> builder)
        {
            builder.ToTable("DocumentTemplates");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.TemplateType, x.CompanyId, x.IsDefault });

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FileResource)
                .WithMany()
                .HasForeignKey(x => x.FileResourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
