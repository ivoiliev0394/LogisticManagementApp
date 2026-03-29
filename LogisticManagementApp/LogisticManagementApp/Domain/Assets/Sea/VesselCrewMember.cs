using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Sea
{
    public class VesselCrewMember : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        public VesselCrewRole CrewRole { get; set; } = VesselCrewRole.Other;

        [MaxLength(50)]
        public string? SeamanBookNumber { get; set; }

        [MaxLength(50)]
        public string? CertificateNumber { get; set; }

        [MaxLength(30)]
        [Phone]
        public string? Phone { get; set; }

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<CrewAssignment> Assignments { get; set; } = new List<CrewAssignment>();
    }
}
