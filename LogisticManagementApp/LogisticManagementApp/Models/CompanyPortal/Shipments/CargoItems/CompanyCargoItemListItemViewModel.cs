namespace LogisticManagementApp.Models.CompanyPortal.Shipments.CargoItems
{
    public class CompanyCargoItemListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public string Description { get; set; } = string.Empty;
        public string CargoItemType { get; set; } = string.Empty;

        public decimal? Quantity { get; set; }
        public string? UnitOfMeasure { get; set; }

        public decimal? GrossWeightKg { get; set; }
        public decimal? NetWeightKg { get; set; }
        public decimal? VolumeCbm { get; set; }

        public decimal? LengthCm { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? HeightCm { get; set; }

        public string? HsCode { get; set; }
        public string? OriginCountry { get; set; }

        public bool IsStackable { get; set; }
        public bool IsFragile { get; set; }

        public string? Notes { get; set; }
    }
}
