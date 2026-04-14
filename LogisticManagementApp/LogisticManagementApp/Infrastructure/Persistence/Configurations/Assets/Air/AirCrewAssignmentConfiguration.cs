using LogisticManagementApp.Domain.Assets.Air;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Air
{
    public class AirCrewAssignmentConfiguration : IEntityTypeConfiguration<AirCrewAssignment>
    {
        public void Configure(EntityTypeBuilder<AirCrewAssignment> builder)
        {
            builder.ToTable("AirCrewAssignments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.FlightId, x.AirCrewMemberId, x.AssignedRole });

            builder.HasOne(x => x.Flight)
                .WithMany(x => x.CrewAssignments)
                .HasForeignKey(x => x.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AirCrewMember)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.AirCrewMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
