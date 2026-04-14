using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class ProofOfDeliveryConfiguration : IEntityTypeConfiguration<ProofOfDelivery>
    {
        public void Configure(EntityTypeBuilder<ProofOfDelivery> builder)
        {
            builder.ToTable("ProofOfDeliveries");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ShipmentId)
                .IsUnique();

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SignatureFileResource)
                .WithMany()
                .HasForeignKey(x => x.SignatureFileResourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
