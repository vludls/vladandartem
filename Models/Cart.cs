using System.Collections.Generic;
using vladandartem.ClassHelpers;

namespace vladandartem.Models
{
    public class Cart
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public List<CartProduct> CartProducts { get; set; }
        public Cart()
        {
            CartProducts = new List<CartProduct>();
        }
    }
}