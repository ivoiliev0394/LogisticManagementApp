using LogisticManagementApp.Domain.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Compliance
{
    public class ComplianceCheckConfiguration : IEntityTypeConfiguration<ComplianceCheck>
    {
        public void Configure(EntityTypeBuilder<ComplianceCheck> builder)
        {
            builder.ToTable("ComplianceChecks");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.CheckType, x.CheckedAtUtc });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
