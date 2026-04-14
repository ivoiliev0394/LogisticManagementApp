using LogisticManagementApp.Domain.Assets.Sea;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Sea
{
    public class VesselCrewMemberConfiguration : IEntityTypeConfiguration<VesselCrewMember>
    {
        public void Configure(EntityTypeBuilder<VesselCrewMember> builder)
        {
            builder.ToTable("VesselCrewMembers");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.CompanyId, x.FullName });

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Assignments)
                .WithOne(x => x.VesselCrewMember)
                .HasForeignKey(x => x.VesselCrewMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
