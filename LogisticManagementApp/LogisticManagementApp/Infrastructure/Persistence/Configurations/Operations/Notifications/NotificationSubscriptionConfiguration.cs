using LogisticManagementApp.Domain.Operations.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Notifications
{
    public class NotificationSubscriptionConfiguration : IEntityTypeConfiguration<NotificationSubscription>
    {
        public void Configure(EntityTypeBuilder<NotificationSubscription> builder)
        {
            builder.ToTable("NotificationSubscriptions");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.UserId, x.EventKey, x.Channel });
            builder.HasIndex(x => new { x.CompanyId, x.EventKey, x.Channel });

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
