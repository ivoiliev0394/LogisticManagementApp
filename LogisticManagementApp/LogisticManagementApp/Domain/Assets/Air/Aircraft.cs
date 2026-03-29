using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Assets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Assets.Air
{
    public class Aircraft : BaseEntity
    {
        [Required]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string TailNumber { get; set; } = null!;

        [Required]
        public AircraftType AircraftType { get; set; } = AircraftType.Freighter;

        [MaxLength(100)]
        public string? Manufacturer { get; set; }

        [MaxLength(100)]
        public string? Model { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxPayloadKg { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? MaxVolumeCbm { get; set; }

        [Required]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        [Required]
        public bool IsActive { get; set; } = true;

        [MaxLength(300)]
        public string? Notes { get; set; }

        public ICollection<Flight> Flights { get; set; } = new List<Flight>();
    }
}
