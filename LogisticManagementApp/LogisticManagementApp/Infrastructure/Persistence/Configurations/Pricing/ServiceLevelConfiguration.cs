using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class ServiceLevelConfiguration : IEntityTypeConfiguration<ServiceLevel>
    {
        public void Configure(EntityTypeBuilder<ServiceLevel> builder)
        {
            builder.ToTable("ServiceLevels");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}
