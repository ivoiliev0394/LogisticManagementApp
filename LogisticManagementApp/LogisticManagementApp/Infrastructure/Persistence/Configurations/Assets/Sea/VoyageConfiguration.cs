using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Sea
{
    public class VoyageConfiguration : IEntityTypeConfiguration<Voyage>
    {
        public void Configure(EntityTypeBuilder<Voyage> builder)
        {
            builder.ToTable("Voyages");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.VesselId, x.VoyageNumber })
                .IsUnique();

            builder.HasOne(x => x.Vessel)
                .WithMany(x => x.Voyages)
                .HasForeignKey(x => x.VesselId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Stops)
                .WithOne(x => x.Voyage)
                .HasForeignKey(x => x.VoyageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
