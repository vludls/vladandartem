using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vladandartem.Models
{
    public class DetailFieldDefinition
    {
        public int Id { get; set; }

        public int DetailFieldId { get; set; }
        public DetailField DetailField { get; set; }

        public int DefinitionId { get; set; }
        public Definition Definition { get; set; }
    }
}
