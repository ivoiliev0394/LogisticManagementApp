using LogisticManagementApp.Domain.Assets.Rail;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Rail
{
    public class TrainConfiguration : IEntityTypeConfiguration<Train>
    {
        public void Configure(EntityTypeBuilder<Train> builder)
        {
            builder.ToTable("Trains");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.TrainNumber)
                .IsUnique();

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.RailMovements)
                .WithOne(x => x.Train)
                .HasForeignKey(x => x.TrainId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
