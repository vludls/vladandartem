using System.ComponentModel.DataAnnotations;

namespace vladandartem.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
 
        [Required(ErrorMessage = "Введите дату рождения")]
        [Display(Name = "Год рождения")]
        public int Year { get; set; }
 
        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
 
        [Required(ErrorMessage = "Введите пароль повторно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль")]
        public string PasswordConfirm { get; set; }
    }
}