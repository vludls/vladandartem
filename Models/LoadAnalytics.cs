using System;
using System.ComponentModel.DataAnnotations;

namespace vladandartem.ViewModels.AdminMenu
{
    public class LoadAnalytics
    {
        public int TimeZoneOffset { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int UserId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int AllTime { get; set; }
        public int LastItemId { get; set; }
    }
}