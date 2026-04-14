using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class PricingQuoteConfiguration : IEntityTypeConfiguration<PricingQuote>
    {
        public void Configure(EntityTypeBuilder<PricingQuote> builder)
        {
            builder.ToTable("PricingQuotes");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.QuoteNumber)
                .IsUnique();

            builder.HasOne(x => x.CustomerCompany)
                .WithMany()
                .HasForeignKey(x => x.CustomerCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Agreement)
                .WithMany(x => x.PricingQuotes)
                .HasForeignKey(x => x.AgreementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ServiceLevel)
                .WithMany()
                .HasForeignKey(x => x.ServiceLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.PricingQuote)
                .HasForeignKey(x => x.PricingQuoteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
