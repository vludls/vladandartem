using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using vladandartem.Models;

namespace vladandartem.ViewModels.Home
{
    public class AddViewModel
    {
        [Required]
        public Product Product { get; set; }

        [Required(ErrorMessage = "Добавьте изображение!")]
        public IFormFile FileImg { get; set;}
        public List<Category> Categories { get; set; }
    }
}