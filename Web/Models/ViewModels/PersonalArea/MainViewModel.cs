using System.Collections.Generic;
using vladandartem.Data.Models;
using vladandartem.Models;

namespace vladandartem.Models.ViewModels.PersonalArea
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