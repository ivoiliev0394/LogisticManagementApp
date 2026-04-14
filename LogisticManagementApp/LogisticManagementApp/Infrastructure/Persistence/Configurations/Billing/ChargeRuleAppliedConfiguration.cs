using LogisticManagementApp.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Billing
{
    public class ChargeRuleAppliedConfiguration : IEntityTypeConfiguration<ChargeRuleApplied>
    {
        public void Configure(EntityTypeBuilder<ChargeRuleApplied> builder)
        {
            builder.ToTable("ChargeRulesApplied");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ChargeId);

            builder.HasOne(x => x.Charge)
                .WithMany()
                .HasForeignKey(x => x.ChargeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
