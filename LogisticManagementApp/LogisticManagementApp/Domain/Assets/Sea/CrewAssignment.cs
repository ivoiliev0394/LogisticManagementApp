using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Sea
{
    public class CrewAssignment : BaseEntity
    {
        [Required]
        public Guid VoyageId { get; set; }

        [ForeignKey(nameof(VoyageId))]
        public Voyage Voyage { get; set; } = null!;

        [Required]
        public Guid VesselCrewMemberId { get; set; }

        [ForeignKey(nameof(VesselCrewMemberId))]
        public VesselCrewMember VesselCrewMember { get; set; } = null!;

        [Required]
        public VesselCrewRole AssignedRole { get; set; } = VesselCrewRole.Other;

        public DateTime? AssignedAtUtc { get; set; }

        public DateTime? FromUtc { get; set; }

        public DateTime? ToUtc { get; set; }

        [MaxLength(300)]
        public string? Notes { get; set; }
    }
}
