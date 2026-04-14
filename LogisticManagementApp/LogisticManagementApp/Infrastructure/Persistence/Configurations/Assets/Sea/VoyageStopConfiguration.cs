using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Sea
{
    public class VoyageStopConfiguration : IEntityTypeConfiguration<VoyageStop>
    {
        public void Configure(EntityTypeBuilder<VoyageStop> builder)
        {
            builder.ToTable("VoyageStops");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.VoyageId, x.SequenceNumber })
                .IsUnique();

            builder.HasOne(x => x.Voyage)
                .WithMany(x => x.Stops)
                .HasForeignKey(x => x.VoyageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
