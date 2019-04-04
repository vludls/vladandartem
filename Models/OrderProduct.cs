using System.Collections.Generic;

namespace vladandartem.Models
{
    public class OrderProduct
    {
        public string Id { get; set; }
        public List<Product> Product { get; set; }
        public string OrderId { get; set; }
        public Order Order { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
    }
}