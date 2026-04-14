using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Sea
{
    public class CrewAssignmentConfiguration : IEntityTypeConfiguration<CrewAssignment>
    {
        public void Configure(EntityTypeBuilder<CrewAssignment> builder)
        {
            builder.ToTable("CrewAssignments");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.VoyageId, x.VesselCrewMemberId, x.AssignedRole });

            builder.HasOne(x => x.Voyage)
                .WithMany()
                .HasForeignKey(x => x.VoyageId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.VesselCrewMember)
                .WithMany(x => x.Assignments)
                .HasForeignKey(x => x.VesselCrewMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
