namespace LogisticManagementApp.Models.CompanyPortal.Shipments.PackageItems
{
    public class CompanyPackageItemListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid PackageId { get; set; }
        public Guid ShipmentId { get; set; }

        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public string? HsCode { get; set; }
        public string? OriginCountry { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Currency { get; set; }
    }
}
