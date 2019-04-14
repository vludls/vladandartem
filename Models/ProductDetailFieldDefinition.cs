using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vladandartem.Models
{
    public class ProductDetailFieldDefinition
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int DetailFieldDefinitionId { get; set; }
        public DetailFieldDefinition DetailFieldDefinition { get; set; }
    }
}
