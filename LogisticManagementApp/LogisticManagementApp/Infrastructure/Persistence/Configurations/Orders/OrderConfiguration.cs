using LogisticManagementApp.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Orders
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.OrderNo)
                .IsUnique();

            builder.HasIndex(x => x.ClientProfileId);

            builder.HasOne(x => x.CustomerCompany)
                .WithMany()
                .HasForeignKey(x => x.CustomerCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ClientProfile)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.ClientProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PickupAddress)
                .WithMany()
                .HasForeignKey(x => x.PickupAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DeliveryAddress)
                .WithMany()
                .HasForeignKey(x => x.DeliveryAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.Order)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
