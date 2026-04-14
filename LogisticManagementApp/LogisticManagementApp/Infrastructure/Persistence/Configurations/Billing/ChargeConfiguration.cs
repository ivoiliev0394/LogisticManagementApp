using LogisticManagementApp.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Billing
{
    public class ChargeConfiguration : IEntityTypeConfiguration<Charge>
    {
        public void Configure(EntityTypeBuilder<Charge> builder)
        {
            builder.ToTable("Charges");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.ChargeCode });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.InvoiceLines)
                .WithOne(x => x.Charge)
                .HasForeignKey(x => x.ChargeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
