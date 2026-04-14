using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentLegConfiguration : IEntityTypeConfiguration<ShipmentLeg>
    {
        public void Configure(EntityTypeBuilder<ShipmentLeg> builder)
        {
            builder.ToTable("ShipmentLegs");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.LegNo })
                .IsUnique();

            builder.HasOne(x => x.Shipment)
                .WithMany(x => x.Legs)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OriginLocation)
                .WithMany()
                .HasForeignKey(x => x.OriginLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationLocation)
                .WithMany()
                .HasForeignKey(x => x.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
