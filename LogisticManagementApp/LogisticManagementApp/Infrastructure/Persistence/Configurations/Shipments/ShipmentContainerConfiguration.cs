using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentContainerConfiguration : IEntityTypeConfiguration<ShipmentContainer>
    {
        public void Configure(EntityTypeBuilder<ShipmentContainer> builder)
        {
            builder.ToTable("ShipmentContainers");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.ContainerId, x.ShipmentLegId });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Container)
                .WithMany(x => x.ShipmentContainers)
                .HasForeignKey(x => x.ContainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
