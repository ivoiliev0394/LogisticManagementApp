using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Road
{
    public class Driver : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        public DriverLicenseCategory LicenseCategory { get; set; } = DriverLicenseCategory.B;

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        [MaxLength(30)]
        [Phone]
        public string? Phone { get; set; }

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
