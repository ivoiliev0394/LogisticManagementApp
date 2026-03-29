using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Operations;
using LogisticManagementApp.Domain.Enums.Shipments;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Operations
{
    public class Consolidation : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string ConsolidationNo { get; set; } = null!;

        [Required]
        public ConsolidationType ConsolidationType { get; set; } = ConsolidationType.Lcl;

        [Required]
        public ConsolidationStatus Status { get; set; } = ConsolidationStatus.Planned;

        [Required]
        public TransportMode TransportMode { get; set; } = TransportMode.Road;

        public DateTime? PlannedDepartureUtc { get; set; }
        public DateTime? PlannedArrivalUtc { get; set; }

        [MaxLength(100)]
        public string? MasterReference { get; set; } // master BL / MAWB / groupage ref

        [MaxLength(500)]
        public string? Notes { get; set; }

        public ICollection<ConsolidationShipment> Shipments { get; set; } = new List<ConsolidationShipment>();
    }
}
