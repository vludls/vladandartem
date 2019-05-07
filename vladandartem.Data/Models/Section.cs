using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vladandartem.Data.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; }

        List<Category> Categories { get; set; }
    }
}
