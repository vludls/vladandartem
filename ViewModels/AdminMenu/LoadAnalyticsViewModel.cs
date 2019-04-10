using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using vladandartem.Models;

namespace vladandartem.ViewModels.AdminMenu
{
    public class LoadAnalyticsViewModel
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