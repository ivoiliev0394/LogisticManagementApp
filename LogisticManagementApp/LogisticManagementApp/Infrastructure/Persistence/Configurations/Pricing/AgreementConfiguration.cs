using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class AgreementConfiguration : IEntityTypeConfiguration<Agreement>
    {
        public void Configure(EntityTypeBuilder<Agreement> builder)
        {
            builder.ToTable("Agreements");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.AgreementNumber)
                .IsUnique();

            builder.HasIndex(x => new { x.CompanyId, x.AgreementType, x.ValidFromUtc, x.ValidToUtc });

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.DiscountRules)
                .WithOne(x => x.Agreement)
                .HasForeignKey(x => x.AgreementId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.PricingQuotes)
                .WithOne(x => x.Agreement)
                .HasForeignKey(x => x.AgreementId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
