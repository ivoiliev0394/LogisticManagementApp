using LogisticManagementApp.Domain.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations
{
    public class ConsolidationShipmentConfiguration : IEntityTypeConfiguration<ConsolidationShipment>
    {
        public void Configure(EntityTypeBuilder<ConsolidationShipment> builder)
        {
            builder.ToTable("ConsolidationShipments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ConsolidationId, x.ShipmentId, x.ShipmentLegId });

            builder.HasOne(x => x.Consolidation)
                .WithMany(x => x.Shipments)
                .HasForeignKey(x => x.ConsolidationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
