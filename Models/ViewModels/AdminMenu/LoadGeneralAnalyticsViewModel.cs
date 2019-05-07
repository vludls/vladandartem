using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace vladandartem.Models.ViewModels.AdminMenu
{
    public class LoadGeneralAnalyticsViewModel
    {
        public int Sales { get; set; }
        public int Revenue { get; set; } // Доход
        public List<MonthState> MonthsState { get; set; } = new List<MonthState>();
    }
}
