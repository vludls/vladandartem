using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using vladandartem.Models;

namespace vladandartem.ViewModels.Home
{
    public class AddViewModel
    {
        [Required]
        public Product product { get; set; }

        [Required(ErrorMessage = "Добавьте изображение!")]
        public IFormFile fileImg { get; set;}
        public IEnumerable<Category> categories { get; set; }
    }
}