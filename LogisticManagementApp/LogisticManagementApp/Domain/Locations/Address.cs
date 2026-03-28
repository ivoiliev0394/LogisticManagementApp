using LogisticManagementApp.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Domain.Locations
{
    public class Address : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = null!; // "Bulgaria"

        [MaxLength(100)]
        public string? Region { get; set; } // област/щат

        [Required]
        [MaxLength(120)]
        public string City { get; set; } = null!;

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = null!;

        [MaxLength(30)]
        public string? Building { get; set; }

        [MaxLength(30)]
        public string? Apartment { get; set; }

        // Координати (за routing/карта)
        [Range(-90, 90)]
        public decimal? Latitude { get; set; }

        [Range(-180, 180)]
        public decimal? Longitude { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
