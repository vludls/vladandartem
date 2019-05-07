using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using vladandartem.Models;
using vladandartem.Data.Models;

namespace vladandartem.Models.ViewModels.Home
{
    public class EditViewModel
    {
        [Required]
        public Product Product { get; set; }

        public List<Category> Categories { get; set; }

        public List<DetailField> DetailFields { get; set; }
    }
}