using LogisticManagementApp.Domain.Assets.Air;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogisticManagementApp.Infrastructure.Persistence.Configurations.Assets.Air
{
    public class AirCrewMemberConfiguration : IEntityTypeConfiguration<AirCrewMember>
    {
        public void Configure(EntityTypeBuilder<AirCrewMember> builder)
        {
            builder.ToTable("AirCrewMembers");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => new { x.CompanyId, x.FullName });

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Assignments)
                .WithOne(x => x.AirCrewMember)
                .HasForeignKey(x => x.AirCrewMemberId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
