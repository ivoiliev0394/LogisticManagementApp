using LogisticManagementApp.Domain.Compliance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Compliance
{
    public class DangerousGoodsDeclarationConfiguration : IEntityTypeConfiguration<DangerousGoodsDeclaration>
    {
        public void Configure(EntityTypeBuilder<DangerousGoodsDeclaration> builder)
        {
            builder.ToTable("DangerousGoodsDeclarations");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ShipmentId, x.PackageId, x.UnNumber });

            builder.HasOne(x => x.Shipment)
                .WithMany()
                .HasForeignKey(x => x.ShipmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Package)
                .WithMany()
                .HasForeignKey(x => x.PackageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
