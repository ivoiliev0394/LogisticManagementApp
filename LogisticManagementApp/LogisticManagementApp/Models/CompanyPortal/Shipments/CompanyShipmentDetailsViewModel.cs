using LogisticManagementApp.Models.CompanyPortal.Shipments.Statuses;

namespace LogisticManagementApp.Models.CompanyPortal.Shipments
{
    public class CompanyShipmentDetailsViewModel
    {
        public Guid Id { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string PrimaryMode { get; set; } = string.Empty;
        public string? Incoterm { get; set; }
        public decimal? DeclaredValue { get; set; }
        public string? Currency { get; set; }
        public string? CustomerReference { get; set; }
        public string? Notes { get; set; }
        public string? OrderNo { get; set; }
        public string? SenderAddress { get; set; }
        public string? ReceiverAddress { get; set; }

        public bool CanEditStatus { get; set; }

        public List<CompanyShipmentPartyItemViewModel> Parties { get; set; } = new();
        public List<CompanyShipmentLegItemViewModel> Legs { get; set; } = new();
        public List<CompanyShipmentStatusHistoryItemViewModel> StatusHistories { get; set; } = new();
        public List<CompanyTrackingEventItemViewModel> TrackingEvents { get; set; } = new();
        public List<CompanyProofOfDeliveryItemViewModel> ProofOfDeliveries { get; set; } = new();
        public List<CompanyPackageItemViewModel> Packages { get; set; } = new();
        public List<CompanyCargoItemItemViewModel> CargoItems { get; set; } = new();
        public List<CompanyShipmentReferenceItemViewModel> References { get; set; } = new();
        public List<CompanyShipmentTagItemViewModel> Tags { get; set; } = new();
        public List<CompanyShipmentVoyageItemViewModel> Voyages { get; set; } = new();
        public List<CompanyShipmentTripItemViewModel> Trips { get; set; } = new();
        public List<CompanyShipmentContainerItemViewModel> Containers { get; set; } = new();
        public List<CompanyShipmentUldItemViewModel> Ulds { get; set; } = new();
    }

    public class CompanyShipmentPartyItemViewModel
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class CompanyShipmentLegItemViewModel
    {
        public Guid Id { get; set; }
        public int LegNo { get; set; }
        public string Mode { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? ETD_Utc { get; set; }
        public DateTime? ETA_Utc { get; set; }
        public DateTime? ATD_Utc { get; set; }
        public DateTime? ATA_Utc { get; set; }
        public string? CarrierReference { get; set; }
    }

    public class CompanyTrackingEventItemViewModel
    {
        public string EventType { get; set; } = string.Empty;
        public DateTime EventTimeUtc { get; set; }
        public string? Location { get; set; }
        public string? Details { get; set; }
        public string? Source { get; set; }
    }

    public class CompanyProofOfDeliveryItemViewModel
    {
        public DateTime DeliveredAtUtc { get; set; }
        public string? ReceiverName { get; set; }
        public string? Notes { get; set; }
    }

    public class CompanyPackageItemViewModel
    {
        public string PackageNo { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;
        public decimal WeightKg { get; set; }
        public decimal? VolumeCbm { get; set; }
        public int ItemsCount { get; set; }
    }

    public class CompanyCargoItemItemViewModel
    {
        public string Description { get; set; } = string.Empty;
        public string CargoItemType { get; set; } = string.Empty;
        public decimal? Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? VolumeCbm { get; set; }
        public bool IsStackable { get; set; }
        public bool IsFragile { get; set; }
    }

    public class CompanyShipmentReferenceItemViewModel
    {
        public string ReferenceType { get; set; } = string.Empty;
        public string ReferenceValue { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

    public class CompanyShipmentTagItemViewModel
    {
        public string TagType { get; set; } = string.Empty;
        public string? CustomValue { get; set; }
        public string? Notes { get; set; }
    }

    public class CompanyShipmentVoyageItemViewModel
    {
        public string VoyageNo { get; set; } = string.Empty;
        public string? BookingReference { get; set; }
        public Guid? ShipmentLegId { get; set; }
    }

    public class CompanyShipmentTripItemViewModel
    {
        public string TripNo { get; set; } = string.Empty;
        public Guid? ShipmentLegId { get; set; }
        public string? Notes { get; set; }
    }

    public class CompanyShipmentContainerItemViewModel
    {
        public string ContainerNo { get; set; } = string.Empty;
        public Guid? ShipmentLegId { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public string? SealNumber { get; set; }
    }

    public class CompanyShipmentUldItemViewModel
    {
        public string UldCode { get; set; } = string.Empty;
        public Guid? ShipmentLegId { get; set; }
        public decimal? GrossWeightKg { get; set; }
        public decimal? VolumeCbm { get; set; }
    }
}
