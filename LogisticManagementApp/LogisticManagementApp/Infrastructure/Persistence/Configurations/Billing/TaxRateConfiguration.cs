using LogisticManagementApp.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Billing
{
    public class TaxRateConfiguration : IEntityTypeConfiguration<TaxRate>
    {
        public void Configure(EntityTypeBuilder<TaxRate> builder)
        {
            builder.ToTable("TaxRates");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.TaxType, x.CountryCode, x.ValidFromUtc, x.ValidToUtc });
        }
    }
}
