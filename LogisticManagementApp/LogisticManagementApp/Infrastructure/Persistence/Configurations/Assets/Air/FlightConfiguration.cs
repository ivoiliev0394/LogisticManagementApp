using LogisticManagementApp.Domain.Assets.Air;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Air
{
    public class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.ToTable("Flights");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.AircraftId, x.FlightNumber, x.ScheduledDepartureUtc });

            builder.HasOne(x => x.Aircraft)
                .WithMany(x => x.Flights)
                .HasForeignKey(x => x.AircraftId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OriginLocation)
                .WithMany()
                .HasForeignKey(x => x.OriginLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationLocation)
                .WithMany()
                .HasForeignKey(x => x.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Segments)
                .WithOne(x => x.Flight)
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.CrewAssignments)
                .WithOne(x => x.Flight)
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
