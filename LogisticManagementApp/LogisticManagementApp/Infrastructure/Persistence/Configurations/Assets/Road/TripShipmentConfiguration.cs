using LogisticManagementApp.Domain.Assets.Road;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Road
{
    public class TripShipmentConfiguration : IEntityTypeConfiguration<TripShipment>
    {
        public void Configure(EntityTypeBuilder<TripShipment> builder)
        {
            builder.ToTable("TripShipments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.TripId, x.ShipmentId, x.ShipmentLegId });

            builder.HasOne(x => x.Trip)
                .WithMany()
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PickupTripStop)
                .WithMany()
                .HasForeignKey(x => x.PickupTripStopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DeliveryTripStop)
                .WithMany()
                .HasForeignKey(x => x.DeliveryTripStopId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
