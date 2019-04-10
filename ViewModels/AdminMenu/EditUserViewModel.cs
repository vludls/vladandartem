using System.ComponentModel.DataAnnotations;

namespace vladandartem.ViewModels.AdminMenu
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        [Display(Name = "��� ��������")]
        public string Year { get; set; }
    }
}