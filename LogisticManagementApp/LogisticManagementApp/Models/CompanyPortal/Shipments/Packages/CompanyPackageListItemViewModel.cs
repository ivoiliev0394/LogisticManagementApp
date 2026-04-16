namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Packages
{
    public class CompanyPackageListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public string PackageNo { get; set; } = string.Empty;
        public string PackageType { get; set; } = string.Empty;

        public decimal WeightKg { get; set; }
        public decimal? LengthCm { get; set; }
        public decimal? WidthCm { get; set; }
        public decimal? HeightCm { get; set; }
        public decimal? VolumeCbm { get; set; }

        public string? Notes { get; set; }
        public int ItemsCount { get; set; }
    }
}
