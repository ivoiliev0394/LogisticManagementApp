using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentTripConfiguration : IEntityTypeConfiguration<ShipmentTrip>
    {
        public void Configure(EntityTypeBuilder<ShipmentTrip> builder)
        {
            builder.ToTable("ShipmentTrips");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.TripId, x.ShipmentLegId });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Trip)
                .WithMany()
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
