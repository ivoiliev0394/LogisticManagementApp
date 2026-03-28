using LogisticManagementApp.Domain.Common;
using LogisticManagementApp.Domain.Enums.Locations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogisticManagementApp.Domain.Locations
{
    public class Location : BaseEntity
    {
        [Required]
        [MaxLength(30)]
        public string Code { get; set; } = null!; // напр. SOF-AIR, VAR-PORT

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        public LocationType LocationType { get; set; } = LocationType.Other;

        [Required]
        public Guid AddressId { get; set; }

        [ForeignKey(nameof(AddressId))]
        public Address Address { get; set; } = null!;

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        // Navigation (ако локацията е склад, ще има Warehouse запис)
        public Warehouse? Warehouse { get; set; }
        public ICollection<Terminal> Terminals { get; set; } = new List<Terminal>();
    }
}
