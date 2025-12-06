
using Application.DTOs.Telemetry;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly TelemetryService _telemetryService;

        public TelemetryController(TelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        [HttpPost("{deviceId:guid}")]
        public async Task<IActionResult> PostTelemetry(Guid deviceId, [FromBody] TelemetryIngestDto data)
        {
            var success = await _telemetryService.IngestTelemetryAsync(deviceId, data);

            if (!success)
            {
                return NotFound(new { message = "Устройство с таким ID не найдено." });
            }

            return NoContent();
        }
    }
}