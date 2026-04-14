using LogisticManagementApp.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Locations
{
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.LocationId).IsUnique();

            builder.HasOne(x => x.Location)
                .WithOne(x => x.Warehouse)
                .HasForeignKey<Warehouse>(x => x.LocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
