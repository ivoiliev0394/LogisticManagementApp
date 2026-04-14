using LogisticManagementApp.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Orders
{
    public class OrderAttachmentConfiguration : IEntityTypeConfiguration<OrderAttachment>
    {
        public void Configure(EntityTypeBuilder<OrderAttachment> builder)
        {
            builder.ToTable("OrderAttachments");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FileResource)
                .WithMany()
                .HasForeignKey(x => x.FileResourceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
