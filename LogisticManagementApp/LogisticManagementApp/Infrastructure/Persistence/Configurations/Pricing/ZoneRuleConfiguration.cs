using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class ZoneRuleConfiguration : IEntityTypeConfiguration<ZoneRule>
    {
        public void Configure(EntityTypeBuilder<ZoneRule> builder)
        {
            builder.ToTable("ZoneRules");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.GeoZoneId, x.Country, x.City, x.PostalCodeFrom, x.PostalCodeTo, x.Priority });

            builder.HasOne(x => x.GeoZone)
                .WithMany(x => x.ZoneRules)
                .HasForeignKey(x => x.GeoZoneId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
