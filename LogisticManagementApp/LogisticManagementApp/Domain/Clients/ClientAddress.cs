using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Orders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Clients
{
    public class ClientAddress : BaseEntity
    {
        [Required]
        public Guid ClientProfileId { get; set; }

        [ForeignKey(nameof(ClientProfileId))]
        public ClientProfile ClientProfile { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PostalCode { get; set; }
        public bool IsDefault { get; set; } = false;

    }
}
