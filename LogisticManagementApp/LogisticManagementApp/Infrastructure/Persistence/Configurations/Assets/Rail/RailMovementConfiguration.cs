using LogisticManagementApp.Domain.Assets.Rail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Rail
{
    public class RailMovementConfiguration : IEntityTypeConfiguration<RailMovement>
    {
        public void Configure(EntityTypeBuilder<RailMovement> builder)
        {
            builder.ToTable("RailMovements");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.MovementNo)
                .IsUnique();

            builder.HasOne(x => x.Train)
                .WithMany(x => x.RailMovements)
                .HasForeignKey(x => x.TrainId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RailService)
                .WithMany(x => x.RailMovements)
                .HasForeignKey(x => x.RailServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.OriginLocation)
                .WithMany()
                .HasForeignKey(x => x.OriginLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationLocation)
                .WithMany()
                .HasForeignKey(x => x.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
