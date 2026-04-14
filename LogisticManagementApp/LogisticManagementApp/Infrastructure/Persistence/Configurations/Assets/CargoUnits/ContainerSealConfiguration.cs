using LogisticManagementApp.Domain.Assets.CargoUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.CargoUnits
{
    public class ContainerSealConfiguration : IEntityTypeConfiguration<ContainerSeal>
    {
        public void Configure(EntityTypeBuilder<ContainerSeal> builder)
        {
            builder.ToTable("ContainerSeals");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ContainerId, x.SealNumber });

            builder.HasOne(x => x.Container)
                .WithMany()
                .HasForeignKey(x => x.ContainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
