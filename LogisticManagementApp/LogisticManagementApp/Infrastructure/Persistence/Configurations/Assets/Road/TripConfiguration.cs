using LogisticManagementApp.Domain.Assets.Road;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Road
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.ToTable("Trips");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.TripNo)
                .IsUnique();

            builder.HasOne(x => x.Vehicle)
                .WithMany(x => x.Trips)
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Driver)
                .WithMany(x => x.Trips)
                .HasForeignKey(x => x.DriverId)
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
