using Newtonsoft.Json;
using System.Collections.Generic;

namespace vladandartem.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int SectionId { get; set; } 
        public Section Section { get; set; }

        public List<Product> Products { get; set; }
    }
}