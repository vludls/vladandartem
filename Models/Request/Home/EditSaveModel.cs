using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace vladandartem.Models.Request.Home
{
    public class EditSaveModel
    {
        [Required]
        public Product Product { get; set; }

        public IFormFile FileImg { get; set; }

        public List<Category> Categories { get; set; }

        public List<DetailField> DetailFields { get; set; }

        public List<int> ProductDetailFieldId { get; set; }
        public List<int?> DefinitionId { get; set; }
    }
}
