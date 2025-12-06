using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Text.Json; 

namespace Application.DTOs.Devices
{
    public class CreateDeviceRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Brand { get; set; }

        public JsonDocument? Config { get; set; }
    }
}
