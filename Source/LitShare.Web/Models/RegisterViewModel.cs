namespace LitShare.Web.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ім'я є обов'язковим")]
        [MaxLength(50, ErrorMessage = "Ім'я не може перевищувати 50 символів")]
        [Display(Name = "Ім'я:")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [MaxLength(255, ErrorMessage = "Email не може перевищувати 255 символів")]
        [Display(Name = "Email:")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Невірний формат телефону")]
        [MaxLength(20, ErrorMessage = "Телефон не може перевищувати 20 символів")]
        [Display(Name = "Телефон:")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Пароль є обов'язковим")]
        [MinLength(8, ErrorMessage = "Пароль повинен містити щонайменше 8 символів")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль:")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Підтвердження паролю є обов'язковим")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Паролі не співпадають")]
        [Display(Name = "Підтвердження паролю:")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Область не може перевищувати 100 символів")]
        [Display(Name = "Область:")]
        public string? Region { get; set; }

        [MaxLength(100, ErrorMessage = "Район не може перевищувати 100 символів")]
        [Display(Name = "Район:")]
        public string? District { get; set; }

        [MaxLength(100, ErrorMessage = "Місто не може перевищувати 100 символів")]
        [Display(Name = "Місто:")]
        public string? City { get; set; }
    }
}