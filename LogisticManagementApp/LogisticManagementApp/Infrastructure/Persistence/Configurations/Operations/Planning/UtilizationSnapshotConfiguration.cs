using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Planning
{
    public class UtilizationSnapshotConfiguration : IEntityTypeConfiguration<UtilizationSnapshot>
    {
        public void Configure(EntityTypeBuilder<UtilizationSnapshot> builder)
        {
            builder.ToTable("UtilizationSnapshots");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ResourceType, x.ResourceId, x.SnapshotDateUtc });
        }
    }
}
