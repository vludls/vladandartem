using vladandartem.ClassHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace vladandartem.Models
{
    public class Order
    {
        public int? Id { get; set; }
        public int Number { get; set; }
        public bool IsPaid { get; set; } = false;
        public int UserId { get; set; }
        public User User { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime OrderTime { get; set; }
        public List<CartProduct> CartProducts { get; set; }
    }
}