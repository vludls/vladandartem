using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace vladandartem.Models.ViewModels.AdminMenu
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "E-mail:")]
        public string Email { get; set; }
        [Display(Name = "Год рождения:")]
        public string Year { get; set; }
        public IList<string> UserRoles { get; set; }
        [Display(Name = "Роли:")]
        public List<IdentityRole<int>> Roles { get; set; }
    }
}