using LogisticManagementApp.Domain.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Compliance
{
    public class TemperatureRequirementConfiguration : IEntityTypeConfiguration<TemperatureRequirement>
    {
        public void Configure(EntityTypeBuilder<TemperatureRequirement> builder)
        {
            builder.ToTable("TemperatureRequirements");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ShipmentId);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
