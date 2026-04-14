using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentReferenceConfiguration : IEntityTypeConfiguration<ShipmentReference>
    {
        public void Configure(EntityTypeBuilder<ShipmentReference> builder)
        {
            builder.ToTable("ShipmentReferences");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.ReferenceType, x.ReferenceValue });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
