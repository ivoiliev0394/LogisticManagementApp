using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class LegStatusHistoryConfiguration : IEntityTypeConfiguration<LegStatusHistory>
    {
        public void Configure(EntityTypeBuilder<LegStatusHistory> builder)
        {
            builder.ToTable("LegStatusHistories");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentLegId, x.ChangedAtUtc });

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
