using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class TariffConfiguration : IEntityTypeConfiguration<Tariff>
    {
        public void Configure(EntityTypeBuilder<Tariff> builder)
        {
            builder.ToTable("Tariffs");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ServiceLevelId, x.GeoZoneId, x.ValidFromUtc, x.ValidToUtc });

            builder.HasOne(x => x.ServiceLevel)
                .WithMany(x => x.Tariffs)
                .HasForeignKey(x => x.ServiceLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.GeoZone)
                .WithMany(x => x.Tariffs)
                .HasForeignKey(x => x.GeoZoneId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Rates)
                .WithOne(x => x.Tariff)
                .HasForeignKey(x => x.TariffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.TariffSurcharges)
                .WithOne(x => x.Tariff)
                .HasForeignKey(x => x.TariffId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
