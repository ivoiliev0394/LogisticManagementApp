using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class TariffRateConfiguration : IEntityTypeConfiguration<TariffRate>
    {
        public void Configure(EntityTypeBuilder<TariffRate> builder)
        {
            builder.ToTable("TariffRates");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.TariffId, x.SortOrder });

            builder.HasOne(x => x.Tariff)
                .WithMany(x => x.Rates)
                .HasForeignKey(x => x.TariffId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
