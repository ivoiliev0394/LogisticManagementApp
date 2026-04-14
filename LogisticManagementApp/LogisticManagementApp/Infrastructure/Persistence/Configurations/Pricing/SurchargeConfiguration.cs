using LogisticManagementApp.Domain.Pricing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Pricing
{
    public class SurchargeConfiguration : IEntityTypeConfiguration<Surcharge>
    {
        public void Configure(EntityTypeBuilder<Surcharge> builder)
        {
            builder.ToTable("Surcharges");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Code)
                .IsUnique();
        }
    }
}
