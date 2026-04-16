namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Containers
{
    public class CompanyShipmentContainerListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public Guid ContainerId { get; set; }
        public Guid? ShipmentLegId { get; set; }

        public string ContainerDisplay { get; set; } = string.Empty;
        public string? ShipmentLegDisplay { get; set; }

        public decimal? GrossWeightKg { get; set; }
        public string? SealNumber { get; set; }
        public string? Notes { get; set; }
    }
}
