using LogisticManagementApp.Domain.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.BookingNo)
                .IsUnique();

            builder.HasIndex(x => new { x.CarrierCompanyId, x.Status, x.TransportMode });

            builder.HasOne(x => x.CarrierCompany)
                .WithMany()
                .HasForeignKey(x => x.CarrierCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Legs)
                .WithOne(x => x.Booking)
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
