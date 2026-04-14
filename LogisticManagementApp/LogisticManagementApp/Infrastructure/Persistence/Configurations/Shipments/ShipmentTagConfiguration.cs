using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentTagConfiguration : IEntityTypeConfiguration<ShipmentTag>
    {
        public void Configure(EntityTypeBuilder<ShipmentTag> builder)
        {
            builder.ToTable("ShipmentTags");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.TagType, x.CustomValue });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
