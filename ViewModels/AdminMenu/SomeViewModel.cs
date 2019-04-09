using System;
using System.ComponentModel.DataAnnotations;

namespace vladandartem.ViewModels.AdminMenu
{
    public class SomeViewModel
    {
        public int TimeZoneOffset { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }

        public string UserEmail { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool AllTime { get; set; }
        public int LastItemId { get; set; }
    }
}