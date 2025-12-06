using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TelemetryData
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public Guid DeviceId { get; set; }
        public Device Device { get; set; } = null!;

        public int GenerationWatts { get; set; }
        public int ConsumptionWatts { get; set; }
        public int GridImportWatts { get; set; }
        public int GridExportWatts { get; set; }
        public int? BatteryPercent { get; set; } 
    }
}