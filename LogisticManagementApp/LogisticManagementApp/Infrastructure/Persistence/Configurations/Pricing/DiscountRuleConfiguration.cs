using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class DiscountRuleConfiguration : IEntityTypeConfiguration<DiscountRule>
    {
        public void Configure(EntityTypeBuilder<DiscountRule> builder)
        {
            builder.ToTable("DiscountRules");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new
            {
                x.AgreementId,
                x.ServiceLevelId,
                x.GeoZoneId,
                x.ValidFromUtc,
                x.ValidToUtc
            });

            builder.HasOne(x => x.Agreement)
                .WithMany(x => x.DiscountRules)
                .HasForeignKey(x => x.AgreementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ServiceLevel)
                .WithMany()
                .HasForeignKey(x => x.ServiceLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.GeoZone)
                .WithMany()
                .HasForeignKey(x => x.GeoZoneId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
