using LogisticManagementApp.Domain.Assets.Rail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Rail
{
    public class RailCarConfiguration : IEntityTypeConfiguration<RailCar>
    {
        public void Configure(EntityTypeBuilder<RailCar> builder)
        {
            builder.ToTable("RailCars");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.RailCarNumber)
                .IsUnique();

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
