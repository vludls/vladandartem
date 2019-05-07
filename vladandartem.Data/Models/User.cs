using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace vladandartem.Data.Models
{
    public class User : IdentityUser<int>
    {
        public string Year { get; set; }

        public List<Order> Order { get; set; }
        public Cart Cart { get; set; }
    }
}