namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Packages
{
    public class CompanyPackagesViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManagePackages { get; set; }

        public IList<CompanyPackageListItemViewModel> Packages { get; set; }
            = new List<CompanyPackageListItemViewModel>();
    }
}
