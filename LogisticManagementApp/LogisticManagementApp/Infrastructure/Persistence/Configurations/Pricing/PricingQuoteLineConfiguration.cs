using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class PricingQuoteLineConfiguration : IEntityTypeConfiguration<PricingQuoteLine>
    {
        public void Configure(EntityTypeBuilder<PricingQuoteLine> builder)
        {
            builder.ToTable("PricingQuoteLines");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.PricingQuoteId, x.LineNo });

            builder.HasOne(x => x.PricingQuote)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.PricingQuoteId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
