using LogisticManagementApp.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Companies
{
    public class CompanyCapabilityConfiguration : IEntityTypeConfiguration<CompanyCapability>
    {
        public void Configure(EntityTypeBuilder<CompanyCapability> builder)
        {
            builder.ToTable("CompanyCapabilities");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.CompanyId);

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
