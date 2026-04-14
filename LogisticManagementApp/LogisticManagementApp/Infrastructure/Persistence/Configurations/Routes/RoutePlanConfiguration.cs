using LogisticManagementApp.Domain.Routes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Routes
{
    public class RoutePlanConfiguration : IEntityTypeConfiguration<RoutePlan>
    {
        public void Configure(EntityTypeBuilder<RoutePlan> builder)
        {
            builder.ToTable("RoutePlans");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.RouteId, x.PlanDateUtc })
                .IsUnique();

            builder.HasOne(x => x.Route)
                .WithMany(x => x.Plans)
                .HasForeignKey(x => x.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Stops)
                .WithOne(x => x.RoutePlan)
                .HasForeignKey(x => x.RoutePlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Shipments)
                .WithOne(x => x.RoutePlan)
                .HasForeignKey(x => x.RoutePlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
