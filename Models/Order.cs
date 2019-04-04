using vladandartem.ClassHelpers;
using System.Collections.Generic;

namespace vladandartem.Models
{
    public class Order
    {
        public string Id { get; set; }
        public int Number { get; set; }
        public bool IsPaid { get; set; } = false;
        public string UserId { get; set; }
        public User User { get; set; }
        public List<CartProduct> CartProducts { get; set; }
    }
}