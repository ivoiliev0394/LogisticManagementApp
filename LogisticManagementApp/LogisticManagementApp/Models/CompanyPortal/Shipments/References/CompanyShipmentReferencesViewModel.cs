namespace LogisticManagementApp.Models.CompanyPortal.Shipments.References
{
    public class CompanyShipmentReferencesViewModel
    {
        public Guid ShipmentId { get; set; }
        public string ShipmentNo { get; set; } = string.Empty;

        public bool CanManageReferences { get; set; }

        public IList<CompanyShipmentReferenceListItemViewModel> References { get; set; }
            = new List<CompanyShipmentReferenceListItemViewModel>();
    }
}
