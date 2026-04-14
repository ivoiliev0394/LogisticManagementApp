using LogisticManagementApp.Domain.Assets.Air;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Air
{
    public class ULDConfiguration : IEntityTypeConfiguration<ULD>
    {
        public void Configure(EntityTypeBuilder<ULD> builder)
        {
            builder.ToTable("ULDs");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.UldNumber)
                .IsUnique();

            builder.HasOne(x => x.OwnerCompany)
                .WithMany()
                .HasForeignKey(x => x.OwnerCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ShipmentULDs)
                .WithOne(x => x.Uld)
                .HasForeignKey(x => x.UldId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
