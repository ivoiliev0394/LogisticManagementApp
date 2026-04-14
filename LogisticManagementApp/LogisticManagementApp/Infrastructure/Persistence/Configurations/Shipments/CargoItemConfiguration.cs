using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class CargoItemConfiguration : IEntityTypeConfiguration<CargoItem>
    {
        public void Configure(EntityTypeBuilder<CargoItem> builder)
        {
            builder.ToTable("CargoItems");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ShipmentId);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
