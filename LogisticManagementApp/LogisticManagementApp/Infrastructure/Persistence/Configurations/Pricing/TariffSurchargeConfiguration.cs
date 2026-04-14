using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class TariffSurchargeConfiguration : IEntityTypeConfiguration<TariffSurcharge>
    {
        public void Configure(EntityTypeBuilder<TariffSurcharge> builder)
        {
            builder.ToTable("TariffSurcharges");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.TariffId, x.SurchargeId, x.ApplyAs });

            builder.HasOne(x => x.Tariff)
                .WithMany(x => x.TariffSurcharges)
                .HasForeignKey(x => x.TariffId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Surcharge)
                .WithMany(x => x.TariffSurcharges)
                .HasForeignKey(x => x.SurchargeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
