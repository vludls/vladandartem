using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vladandartem.Data.Models
{
    public class ProductDetailField
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int? DefinitionId { get; set; }

        public int DetailFieldId { get; set; }
        public DetailField DetailField { get; set; }
    }
}
