using LogisticManagementApp.Domain.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Compliance
{
    public class DGDocumentConfiguration : IEntityTypeConfiguration<DGDocument>
    {
        public void Configure(EntityTypeBuilder<DGDocument> builder)
        {
            builder.ToTable("DGDocuments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.DangerousGoodsDeclarationId, x.FileResourceId });

            builder.HasOne(x => x.DangerousGoodsDeclaration)
                .WithMany()
                .HasForeignKey(x => x.DangerousGoodsDeclarationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FileResource)
                .WithMany()
                .HasForeignKey(x => x.FileResourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
