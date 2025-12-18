using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Application.DTOs.Statistics
{
    public class DailyStatisticsDto 
    {
        public DateTime Date { get; set; }
        public double TotalGenerationWh { get; set; } 
        public double TotalConsumptionWh { get; set; }
        public double MoneySaved { get; set; }
        public double SelfSufficiencyPercent { get; set; }
        public double NetGridBalanceWh { get; set; }
    }
}