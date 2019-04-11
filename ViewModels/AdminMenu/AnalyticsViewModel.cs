using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using vladandartem.Models;

namespace vladandartem.ViewModels.AdminMenu
{
    public class AnalyticsViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<User> Users { get; set; }
    }
}