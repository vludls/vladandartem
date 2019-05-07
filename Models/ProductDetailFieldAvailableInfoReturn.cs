using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vladandartem.Data.Models;

namespace vladandartem.Models
{
    public class ProductDetailFieldAvailableInfoReturn
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int? DefinitionId { get; set; }

        public int DetailFieldId { get; set; }
        public DetailField DetailField { get; set; }
    }
}
