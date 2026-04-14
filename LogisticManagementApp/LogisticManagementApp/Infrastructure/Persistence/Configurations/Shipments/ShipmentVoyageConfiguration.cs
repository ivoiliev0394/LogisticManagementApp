using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentVoyageConfiguration : IEntityTypeConfiguration<ShipmentVoyage>
    {
        public void Configure(EntityTypeBuilder<ShipmentVoyage> builder)
        {
            builder.ToTable("ShipmentVoyages");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.VoyageId, x.ShipmentLegId });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Voyage)
                .WithMany()
                .HasForeignKey(x => x.VoyageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
