using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Companies;
using LogisticManagementApp.Domain.Identity;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Companies
{
    public class Company : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// VAT / EIK / Tax number (според държава). Не правя Unique тук с атрибут, защото често има null/празни.
        /// Уникалност ще се прави с Fluent API по-късно, ако искаш.
        /// </summary>
        [MaxLength(50)]
        public string? TaxNumber { get; set; }

        [Required]
        public CompanyType CompanyType { get; set; } = CompanyType.Other;

        [MaxLength(200)]
        public string? Website { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        public ApplicationUser? User { get; set; }

        // Navigation
        public ICollection<CompanyContact> Contacts { get; set; } = new List<CompanyContact>();
        public ICollection<CompanyBranch> Branches { get; set; } = new List<CompanyBranch>();
        
    }
}
