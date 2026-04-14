using LogisticManagementApp.Domain.Operations.Preferences;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Preferences
{
    public class SavedFilterConfiguration : IEntityTypeConfiguration<SavedFilter>
    {
        public void Configure(EntityTypeBuilder<SavedFilter> builder)
        {
            builder.ToTable("SavedFilters");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.UserId, x.EntityType, x.Name });
            builder.HasIndex(x => new { x.CompanyId, x.EntityType, x.Name });

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
