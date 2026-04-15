using System.ComponentModel.DataAnnotations;

namespace LogisticManagementApp.Models.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Полето е задължително.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Името трябва да е между 2 и 50 символа.")]
        [Display(Name = "Първо име")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полето е задължително.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Фамилията трябва да е между 2 и 50 символа.")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полето е задължително.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username трябва да е между 3 и 50 символа.")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полето е задължително.")]
        [EmailAddress(ErrorMessage = "Невалиден email адрес.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Невалиден телефонен номер.")]
        [StringLength(20, ErrorMessage = "Телефонният номер трябва да е до 20 символа.")]
        [Display(Name = "Телефон")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Полето е задължително.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Паролата трябва да е поне 6 символа.")]
        [Display(Name = "Парола")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полето е задължително.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
        [Display(Name = "Потвърди парола")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}