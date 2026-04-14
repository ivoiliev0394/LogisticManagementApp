using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentULDConfiguration : IEntityTypeConfiguration<ShipmentULD>
    {
        public void Configure(EntityTypeBuilder<ShipmentULD> builder)
        {
            builder.ToTable("ShipmentULDs");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.UldId, x.ShipmentLegId });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Uld)
                .WithMany(x => x.ShipmentULDs)
                .HasForeignKey(x => x.UldId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
