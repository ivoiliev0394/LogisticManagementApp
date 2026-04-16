namespace LogisticManagementApp.Models.CompanyPortal.Contacts
{
    public class CompanyContactListItemViewModel
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = null!;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? RoleTitle { get; set; }

        public bool IsPrimary { get; set; }
    }
}
