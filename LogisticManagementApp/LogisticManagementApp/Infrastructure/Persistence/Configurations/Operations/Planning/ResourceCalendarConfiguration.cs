using LogisticManagementApp.Domain.Operations.Planning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Operations.Planning
{
    public class ResourceCalendarConfiguration : IEntityTypeConfiguration<ResourceCalendar>
    {
        public void Configure(EntityTypeBuilder<ResourceCalendar> builder)
        {
            builder.ToTable("ResourceCalendars");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.ResourceType, x.ResourceId, x.DateUtc })
                .IsUnique();
        }
    }
}
