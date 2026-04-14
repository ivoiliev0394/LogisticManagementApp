using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Planning
{
    public class CapacityReservationConfiguration : IEntityTypeConfiguration<CapacityReservation>
    {
        public void Configure(EntityTypeBuilder<CapacityReservation> builder)
        {
            builder.ToTable("CapacityReservations");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ResourceType, x.ResourceId, x.ShipmentId, x.ShipmentLegId });

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
