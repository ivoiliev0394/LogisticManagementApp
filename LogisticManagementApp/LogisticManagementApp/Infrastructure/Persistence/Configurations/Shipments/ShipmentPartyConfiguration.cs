using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ShipmentPartyConfiguration : IEntityTypeConfiguration<ShipmentParty>
    {
        public void Configure(EntityTypeBuilder<ShipmentParty> builder)
        {
            builder.ToTable("ShipmentParties");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.CompanyId, x.Role });

            builder.HasOne(x => x.Shipment)
                .WithMany(x => x.Parties)
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
