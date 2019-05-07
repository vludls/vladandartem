using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vladandartem.Data.Models
{
    public class Definition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DetailFieldId { get; set; }
        public DetailField DetailField { get; set; }
    }
}
