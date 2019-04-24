
using System.Collections.Generic;
using vladandartem.Models;

namespace vladandartem.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public int Page { get; set; }
        public int PagesCount { get; set; }
        public string SearchArgument { get; set; }
    }
}