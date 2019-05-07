using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using vladandartem.Models;
using vladandartem.Data.Models;
using vladandartem.Models.ViewModels.AdminMenu;

namespace vladandartem.Models.ViewModels.AdminMenu
{
    public class AnalyticsViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<User> Users { get; set; }
    }
}