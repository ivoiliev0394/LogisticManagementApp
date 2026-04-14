using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Planning
{
    public class ResourceAvailabilityConfiguration : IEntityTypeConfiguration<ResourceAvailability>
    {
        public void Configure(EntityTypeBuilder<ResourceAvailability> builder)
        {
            builder.ToTable("ResourceAvailabilities");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ResourceType, x.ResourceId, x.AvailableFromUtc, x.AvailableToUtc });
        }
    }
}
