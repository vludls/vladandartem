using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using vladandartem.Models;

namespace vladandartem.ViewModels.Home
{
    public class EditViewModel
    {
        [Required]
        public Product Product { get; set; }

        public List<Category> Categories { get; set; }

        public List<DetailField> DetailFields { get; set; }
    }
}