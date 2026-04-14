using LogisticManagementApp.Domain.Assets.Road;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Road
{
    public class TripStopConfiguration : IEntityTypeConfiguration<TripStop>
    {
        public void Configure(EntityTypeBuilder<TripStop> builder)
        {
            builder.ToTable("TripStops");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.TripId, x.SequenceNumber })
                .IsUnique();

            builder.HasOne(x => x.Trip)
                .WithMany()
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
