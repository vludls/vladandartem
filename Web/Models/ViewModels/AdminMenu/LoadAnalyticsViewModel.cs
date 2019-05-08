using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using vladandartem.Models;
using vladandartem.Data.Models;

namespace vladandartem.Models.ViewModels.AdminMenu
{
    public class ProductAnalytics
    {
        public Product Product { get; set; }
        public int Sales { get; set; }
        public int Revenue { get; set; } // Доход
        public List<MonthState> MonthsState { get; set; } = new List<MonthState>();
    }

    public class MonthState
    {
        public DateTime Month { get; set; }
        public List<DayState> Days { get; set; } = new List<DayState>();

        public MonthState() { }

        public MonthState(DateTime time)
        {
            Month = time;
        }
    }

    public class DayState
    {
        public DateTime Day { get; set; }
        public int Sales { get; set; }
        public int Revenue { get; set; }
        public DayState(DateTime time)
        {
            Day = time;
        }

    }
}