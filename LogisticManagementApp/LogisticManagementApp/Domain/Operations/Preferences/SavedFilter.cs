using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Operations.Preferences
{
    public class SavedFilter : BaseEntity
    {
        [MaxLength(450)]
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public Guid? CompanyId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = null!; // Shipments, Orders, Invoices...

        [Required]
        [MaxLength(4000)]
        public string FilterJson { get; set; } = null!;
        [Required]
        public bool IsDefault { get; set; } = false;
    }
}