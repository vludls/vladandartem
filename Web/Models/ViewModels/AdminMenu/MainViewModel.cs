using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vladandartem.Models;
using vladandartem.Data.Models;

namespace vladandartem.Models.ViewModels.AdminMenu
{
    public class MainViewModel
    {
        public List<Section> Sections { get; set; }
        public List<Category> Categories { get; set; }
    }
}
