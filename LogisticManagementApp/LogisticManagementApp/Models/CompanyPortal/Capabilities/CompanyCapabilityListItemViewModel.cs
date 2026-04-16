namespace LogisticManagementApp.Models.CompanyPortal.Capabilities
{
    public class CompanyCapabilityListItemViewModel
    {
        public Guid Id { get; set; }

        public string CapabilityType { get; set; } = null!;

        public bool IsEnabled { get; set; }

        public DateTime? ValidFromUtc { get; set; }

        public DateTime? ValidToUtc { get; set; }

        public string? Notes { get; set; }
    }
}
