using LogisticManagementApp.Domain.Routes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Routes
{
    public class RoutePlanShipmentConfiguration : IEntityTypeConfiguration<RoutePlanShipment>
    {
        public void Configure(EntityTypeBuilder<RoutePlanShipment> builder)
        {
            builder.ToTable("RoutePlanShipments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.RoutePlanId, x.ShipmentId });

            builder.HasOne(x => x.RoutePlan)
                .WithMany(x => x.Shipments)
                .HasForeignKey(x => x.RoutePlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PickupStop)
                .WithMany()
                .HasForeignKey(x => x.PickupStopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DeliveryStop)
                .WithMany()
                .HasForeignKey(x => x.DeliveryStopId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
