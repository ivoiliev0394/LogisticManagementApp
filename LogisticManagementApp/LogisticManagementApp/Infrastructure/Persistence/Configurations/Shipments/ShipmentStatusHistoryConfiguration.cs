using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentStatusHistoryConfiguration : IEntityTypeConfiguration<ShipmentStatusHistory>
    {
        public void Configure(EntityTypeBuilder<ShipmentStatusHistory> builder)
        {
            builder.ToTable("ShipmentStatusHistories");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.ChangedAtUtc });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
