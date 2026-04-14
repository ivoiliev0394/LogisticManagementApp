using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Planning
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.ToTable("Assignments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentLegId, x.ResourceType, x.ResourceId });

            builder.HasOne(x => x.ShipmentLeg)
                .WithMany()
                .HasForeignKey(x => x.ShipmentLegId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
