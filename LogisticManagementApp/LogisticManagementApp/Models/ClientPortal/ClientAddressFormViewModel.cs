using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.ClientPortal
{
    public class ClientAddressFormViewModel
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Държавата е задължителна.")]
        [MaxLength(100, ErrorMessage = "Държавата не може да е повече от 100 символа.")]
        [Display(Name = "Държава")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Градът е задължителен.")]
        [MaxLength(100, ErrorMessage = "Градът не може да е повече от 100 символа.")]
        [Display(Name = "Град")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Улицата е задължителна.")]
        [MaxLength(200, ErrorMessage = "Улицата не може да е повече от 200 символа.")]
        [Display(Name = "Улица")]
        public string Street { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Пощенският код не може да е повече от 20 символа.")]
        [Display(Name = "Пощенски код")]
        public string? PostalCode { get; set; }

        [Display(Name = "Основен адрес")]
        public bool IsDefault { get; set; }
    }
}
