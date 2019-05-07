using System.Collections.Generic;

namespace vladandartem.Data.Models
{
    public class Cart
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        //[JsonIgnore]
        public User User { get; set; }
        public List<CartProduct> CartProducts { get; set; } = new List<CartProduct>();
    }
}