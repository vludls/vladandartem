
using System.Collections.Generic;
using vladandartem.Models;

namespace vladandartem.ViewModels.Home
{
    public class IndexViewModel
    {
        public IEnumerable<Product> products { get; set; }
        public int page { get; set; }
        public int pagesCount { get; set; }

        public string searchArgument { get; set; }
    }
}