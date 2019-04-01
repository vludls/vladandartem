using System.Collections.Generic;
using vladandartem.ClassHelpers;
using vladandartem.Models;

namespace vladandartem.ViewModels.PersonalArea
{
    public class PaidProduct
    {
        public Product product { get; set; }
        public int count { get; set; }
    }
    public class MainViewModel
    {    
        public IEnumerable<CartProduct> paidProducts { get; set; }
    }
}