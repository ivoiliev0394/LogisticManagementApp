using LogisticManagementApp.Domain.Routes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Routes
{
    public class RoutePlanStopConfiguration : IEntityTypeConfiguration<RoutePlanStop>
    {
        public void Configure(EntityTypeBuilder<RoutePlanStop> builder)
        {
            builder.ToTable("RoutePlanStops");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.RoutePlanId, x.SequenceNo })
                .IsUnique();

            builder.HasOne(x => x.RoutePlan)
                .WithMany(x => x.Stops)
                .HasForeignKey(x => x.RoutePlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RouteStop)
                .WithMany()
                .HasForeignKey(x => x.RouteStopId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Location)
                .WithMany()
                .HasForeignKey(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
