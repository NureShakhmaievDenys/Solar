using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Devices
{
    public class DeviceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public Guid SiteId { get; set; }
        public int? CurrentPowerWatts { get; set; }

        public bool IsOnline { get; set; }
    }
}
