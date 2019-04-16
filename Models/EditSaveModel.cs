using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace vladandartem.Models
{
    public class EditSaveModel
    {
        [Required]
        public Product Product { get; set; }

        public List<Category> Categories { get; set; }

        public List<DetailField> DetailFields { get; set; }

        public List<ProductDFDefinition> ProductDFDefinitions { get; set; }
    }

    public class ProductDFDefinition
    {
        public int ProductDetailFieldId { get; set; }
        public int DefinitionId { get; set; }
    }
}
