using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace vladandartem.Data.Models
{
    public class CartProduct
    {
        public int Id { get; set; }
        public int? CartId { get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }

        public int? OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Count { get; set; }
    }
}
