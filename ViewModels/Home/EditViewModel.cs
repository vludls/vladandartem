using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using vladandartem.Models;

namespace vladandartem.ViewModels.Home
{
    public class EditViewModel
    {
        [Required]
        public Product product { get; set; }

        public IEnumerable<Category> categories { get; set; }
    }
}