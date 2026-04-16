namespace LogisticManagementApp.Models.CompanyPortal.Shipments.PackageItems
{
    public class CompanyPackageItemsViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public Guid PackageId { get; set; }
        public string PackageNo { get; set; } = string.Empty;

        public bool CanManagePackageItems { get; set; }

        public IList<CompanyPackageItemListItemViewModel> PackageItems { get; set; }
            = new List<CompanyPackageItemListItemViewModel>();
    }
}
