using LogisticManagementApp.Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Shipments
{
    public class PackageItemConfiguration : IEntityTypeConfiguration<PackageItem>
    {
        public void Configure(EntityTypeBuilder<PackageItem> builder)
        {
            builder.ToTable("PackageItems");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.PackageId);

            builder.HasOne(x => x.Package)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
