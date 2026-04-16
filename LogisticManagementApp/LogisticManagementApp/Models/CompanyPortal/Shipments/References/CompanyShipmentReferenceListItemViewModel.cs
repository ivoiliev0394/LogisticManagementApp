namespace LogisticManagementApp.Models.CompanyPortal.Shipments.References
{
    public class CompanyShipmentReferenceListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public string ReferenceType { get; set; } = string.Empty;
        public string ReferenceValue { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
