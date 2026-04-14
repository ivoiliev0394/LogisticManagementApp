using LogisticManagementApp.Domain.Assets.Rail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Rail
{
    public class RailServiceConfiguration : IEntityTypeConfiguration<RailService>
    {
        public void Configure(EntityTypeBuilder<RailService> builder)
        {
            builder.ToTable("RailServices");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.ServiceCode)
                .IsUnique();

            builder.HasOne(x => x.OriginLocation)
                .WithMany()
                .HasForeignKey(x => x.OriginLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DestinationLocation)
                .WithMany()
                .HasForeignKey(x => x.DestinationLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.RailMovements)
                .WithOne(x => x.RailService)
                .HasForeignKey(x => x.RailServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
