using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vladandartem.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int SectionId { get; set; }
        [ForeignKey("SectionId")]
        public Section Section { get; set; }

        public List<Product> Products { get; set; }
    }
}