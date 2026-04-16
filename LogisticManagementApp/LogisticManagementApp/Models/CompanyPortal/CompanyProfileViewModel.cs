namespace LogisticManagementApp.Models.CompanyPortal
{
    public class CompanyProfileViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? TaxNumber { get; set; }
        public string CompanyType { get; set; } = null!;
        public string? Website { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}
