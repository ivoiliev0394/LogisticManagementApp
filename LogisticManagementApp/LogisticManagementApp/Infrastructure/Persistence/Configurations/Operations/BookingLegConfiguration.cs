using LogisticManagementApp.Domain.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations
{
    public class BookingLegConfiguration : IEntityTypeConfiguration<BookingLeg>
    {
        public void Configure(EntityTypeBuilder<BookingLeg> builder)
        {
            builder.ToTable("BookingLegs");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.BookingId, x.LegNo })
                .IsUnique();

            builder.HasOne(x => x.Booking)
                .WithMany(x => x.Legs)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
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
