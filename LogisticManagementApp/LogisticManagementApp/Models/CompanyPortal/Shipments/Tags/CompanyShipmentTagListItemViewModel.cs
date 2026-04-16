namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Tags
{
    public class CompanyShipmentTagListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public string TagType { get; set; } = string.Empty;
        public string? CustomValue { get; set; }
        public string? Notes { get; set; }
    }
}
