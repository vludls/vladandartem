using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace vladandartem.Models.Request.AdminMenu
{
    public class UserSaveModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Year { get; set; }

        public List<string> Roles { get; set; }
    }
}
