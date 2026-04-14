using LogisticManagementApp.Domain.Documents;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Documents
{
    public class DocumentConfiguration : IEntityTypeConfiguration<Document>
    {
        public void Configure(EntityTypeBuilder<Document> builder)
        {
            builder.ToTable("Documents");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.DocumentType, x.DocumentNo });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FileResource)
                .WithMany()
                .HasForeignKey(x => x.FileResourceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.IssuedByCompany)
                .WithMany()
                .HasForeignKey(x => x.IssuedByCompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
