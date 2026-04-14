using LogisticManagementApp.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Documents
{
    public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
    {
        public void Configure(EntityTypeBuilder<DocumentVersion> builder)
        {
            builder.ToTable("DocumentVersions");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.DocumentId, x.VersionNo })
                .IsUnique();

            builder.HasOne(x => x.Document)
                .WithMany()
                .HasForeignKey(x => x.DocumentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FileResource)
                .WithMany()
                .HasForeignKey(x => x.FileResourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
