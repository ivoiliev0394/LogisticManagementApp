using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Полето е задължително.")]
        [Display(Name = "Username или Email")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полето е задължително.")]
        [DataType(DataType.Password)]
        [Display(Name = "Парола")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запомни ме")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}