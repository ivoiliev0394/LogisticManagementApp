using LogisticManagementApp.Domain.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations
{
    public class ConsolidationConfiguration : IEntityTypeConfiguration<Consolidation>
    {
        public void Configure(EntityTypeBuilder<Consolidation> builder)
        {
            builder.ToTable("Consolidations");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ConsolidationNo)
                .IsUnique();

            builder.HasMany(x => x.Shipments)
                .WithOne(x => x.Consolidation)
                .HasForeignKey(x => x.ConsolidationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
