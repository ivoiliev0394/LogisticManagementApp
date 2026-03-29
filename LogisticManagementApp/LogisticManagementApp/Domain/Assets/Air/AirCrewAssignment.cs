using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Air
{
    public class AirCrewAssignment : BaseEntity
    {
        [Required]
        public Guid FlightId { get; set; }

        [ForeignKey(nameof(FlightId))]
        public Flight Flight { get; set; } = null!;

        [Required]
        public Guid AirCrewMemberId { get; set; }

        [ForeignKey(nameof(AirCrewMemberId))]
        public AirCrewMember AirCrewMember { get; set; } = null!;

        [Required]
        public AirCrewRole AssignedRole { get; set; } = AirCrewRole.Other;

        public DateTime? AssignedAtUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
