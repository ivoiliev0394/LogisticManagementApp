using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.CompanyPortal.Branches
{
    public class CompanyBranchEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = null!;

        public Guid? AddressId { get; set; }

        [StringLength(50)]
        public string? BranchCode { get; set; }

        [StringLength(50)]
        public string? Phone { get; set; }

        [EmailAddress]
        [StringLength(120)]
        public string? Email { get; set; }

        public bool IsHeadOffice { get; set; }

        public bool IsActive { get; set; }

        [StringLength(300)]
        public string? Notes { get; set; }

        public IEnumerable<SelectListItem> AddressOptions { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
