namespace LogisticManagementApp.Models.CompanyPortal.Branches
{
    public class CompanyBranchListItemViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? BranchCode { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public Guid? AddressId { get; set; }

        public string? AddressText { get; set; }

        public bool IsHeadOffice { get; set; }

        public bool IsActive { get; set; }

        public string? Notes { get; set; }
    }
}
