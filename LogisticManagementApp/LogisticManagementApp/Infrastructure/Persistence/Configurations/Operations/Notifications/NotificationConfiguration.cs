using LogisticManagementApp.Domain.Operations.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Notifications
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.RecipientUserId, x.IsRead });
            builder.HasIndex(x => new { x.RecipientCompanyId, x.NotificationType });
            builder.HasIndex(x => new { x.RelatedEntityType, x.RelatedEntityId });

            builder.HasOne(x => x.RecipientUser)
                .WithMany()
                .HasForeignKey(x => x.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
