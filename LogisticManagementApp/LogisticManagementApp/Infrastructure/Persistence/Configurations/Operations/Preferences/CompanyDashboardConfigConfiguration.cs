using LogisticManagementApp.Domain.Operations.Preferences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Preferences
{
    public class CompanyDashboardConfigConfiguration : IEntityTypeConfiguration<CompanyDashboardConfig>
    {
        public void Configure(EntityTypeBuilder<CompanyDashboardConfig> builder)
        {
            builder.ToTable("CompanyDashboardConfigs");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.CompanyId, x.DashboardKey })
                .IsUnique();

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
