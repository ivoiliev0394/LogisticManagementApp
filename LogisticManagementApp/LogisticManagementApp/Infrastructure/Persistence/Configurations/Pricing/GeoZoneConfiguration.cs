using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class GeoZoneConfiguration : IEntityTypeConfiguration<GeoZone>
    {
        public void Configure(EntityTypeBuilder<GeoZone> builder)
        {
            builder.ToTable("GeoZones");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}
