using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Identity;
using LogisticManagementApp.Domain.Orders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Clients
{
    public class ClientProfile : BaseEntity
    {
        [Required]
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AspNetUsers? User { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(100)]
        public string? EmailForContact { get; set; }

        [Required]
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        public ICollection<ClientAddress> Addresses { get; set; } = new List<ClientAddress>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
