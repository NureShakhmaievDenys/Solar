using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Core/Entities/Device.cs
using System.Text.Json; // Потрібно для JsonDocument

namespace Core.Entities
{
    public class Device
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Brand { get; set; }

        public Guid SiteId { get; set; }
        public Site Site { get; set; } = null!;
        public ICollection<TelemetryData> TelemetryData { get; set; } = new List<TelemetryData>();
        public string? Config { get; set; }
    }
}
