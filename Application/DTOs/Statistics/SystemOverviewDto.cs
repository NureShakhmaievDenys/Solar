using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Statistics
{
    public class SystemOverviewDto
    {
        public int TotalUsers { get; set; }
        public int TotalSites { get; set; }
        public int ActiveDevices { get; set; } 
        public long TotalTelemetryRecords { get; set; }
        
    }
}
