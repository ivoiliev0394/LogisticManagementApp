namespace LogisticManagementApp.Models.CompanyPortal.Shipments.Party
{
    public class CompanyShipmentPartyListItemViewModel
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }

        public Guid CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public Guid? CompanyContactId { get; set; }
        public string? CompanyContactName { get; set; }

    }
}
