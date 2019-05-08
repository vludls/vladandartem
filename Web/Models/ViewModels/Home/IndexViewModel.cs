
using System.Collections.Generic;
using vladandartem.Models;
using vladandartem.Data.Models;

namespace vladandartem.Models.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public int Page { get; set; }
        public int PagesCount { get; set; }
        public string SearchArgument { get; set; }
    }
}