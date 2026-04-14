using LogisticManagementApp.Domain.Assets.CargoUnits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.CargoUnits
{
    public class ContainerConfiguration : IEntityTypeConfiguration<Container>
    {
        public void Configure(EntityTypeBuilder<Container> builder)
        {
            builder.ToTable("Containers");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ContainerNumber)
                .IsUnique();

            builder.HasOne(x => x.OwnerCompany)
                .WithMany()
                .HasForeignKey(x => x.OwnerCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.ShipmentContainers)
                .WithOne(x => x.Container)
                .HasForeignKey(x => x.ContainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
