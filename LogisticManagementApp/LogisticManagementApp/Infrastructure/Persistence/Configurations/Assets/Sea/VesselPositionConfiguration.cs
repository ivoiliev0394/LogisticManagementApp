using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Sea
{
    public class VesselPositionConfiguration : IEntityTypeConfiguration<VesselPosition>
    {
        public void Configure(EntityTypeBuilder<VesselPosition> builder)
        {
            builder.ToTable("VesselPositions");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.VesselId, x.PositionTimeUtc });

            builder.HasOne(x => x.Vessel)
                .WithMany()
                .HasForeignKey(x => x.VesselId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
