using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.DTOs.Dashboard
{
    public class RealtimeDashboardDto
    {
        public Guid SiteId { get; set; }
        public DateTime Timestamp { get; set; }
        public int GenerationWatts { get; set; }
        public int ConsumptionWatts { get; set; }
        public int GridImportWatts { get; set; }
        public int GridExportWatts { get; set; }

        public int? BatteryPercent { get; set; }
        public int? BatteryPowerWatts { get; set; }
    }
}