namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Ulds
{
    public class CompanyShipmentUldListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public Guid UldId { get; set; }
        public Guid? ShipmentLegId { get; set; }

        public string UldDisplay { get; set; } = string.Empty;
        public string? ShipmentLegDisplay { get; set; }

        public decimal? GrossWeightKg { get; set; }
        public decimal? VolumeCbm { get; set; }
        public string? Notes { get; set; }
    }
}
