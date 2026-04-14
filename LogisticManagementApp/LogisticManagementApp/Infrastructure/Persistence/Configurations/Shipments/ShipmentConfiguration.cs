using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.ToTable("Shipments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ShipmentNo)
                .IsUnique();

            builder.HasOne(x => x.CustomerCompany)
                .WithMany()
                .HasForeignKey(x => x.CustomerCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SenderAddress)
                .WithMany()
                .HasForeignKey(x => x.SenderAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ReceiverAddress)
                .WithMany()
                .HasForeignKey(x => x.ReceiverAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Parties)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Legs)
                .WithOne(x => x.Shipment)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
