using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vladandartem.Models
{
    public class DetailField
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<DetailFieldDefinition> DetailFieldDefinitions { get; set; }
    }
}
