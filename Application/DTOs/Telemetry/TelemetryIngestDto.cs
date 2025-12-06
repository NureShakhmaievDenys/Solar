using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Telemetry
{
    public class TelemetryIngestDto
    {
        public DateTime? Timestamp { get; set; }

        [Required]
        public int GenerationWatts { get; set; }
        [Required]
        public int ConsumptionWatts { get; set; }

        public int GridImportWatts { get; set; } = 0;
        public int GridExportWatts { get; set; } = 0;
        public int? BatteryPercent { get; set; }
    }
}