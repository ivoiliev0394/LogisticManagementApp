using LogisticManagementApp.Domain.Assets.Air;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Air
{
    public class FlightSegmentConfiguration : IEntityTypeConfiguration<FlightSegment>
    {
        public void Configure(EntityTypeBuilder<FlightSegment> builder)
        {
            builder.ToTable("FlightSegments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.FlightId, x.SegmentNo })
                .IsUnique();

            builder.HasOne(x => x.Flight)
                .WithMany(x => x.Segments)
                .HasForeignKey(x => x.FlightId)
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
