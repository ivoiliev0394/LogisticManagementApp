using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Sea
{
    public class VesselConfiguration : IEntityTypeConfiguration<Vessel>
    {
        public void Configure(EntityTypeBuilder<Vessel> builder)
        {
            builder.ToTable("Vessels");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ImoNumber)
                .IsUnique();

            builder.HasIndex(x => x.MmsiNumber)
                .IsUnique();

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Voyages)
                .WithOne(x => x.Vessel)
                .HasForeignKey(x => x.VesselId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
