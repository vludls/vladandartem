using System;
using System.ComponentModel.DataAnnotations;

namespace vladandartem.Models.ViewModels.AdminMenu
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
        [DataType(DataType.Date)]
        public DateTime DateFrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateTo { get; set; }
        public int AllTime { get; set; }
        public int LastItemId { get; set; }
    }
}