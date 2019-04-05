using System.ComponentModel.DataAnnotations;

namespace vladandartem.ViewModels.AdminMenu
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Введите Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Дата рождения")]
        public string Year { get; set; }
 
        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}