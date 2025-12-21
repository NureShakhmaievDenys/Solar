using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly StatisticsService _statsService;

        public StatisticsController(StatisticsService statsService)
        {
            _statsService = statsService;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdString!);
        }

        // 1. АДМИНСКАЯ ПАНЕЛЬ
        // GET /api/statistics/admin/overview
        [HttpGet("admin/overview")]
        public async Task<IActionResult> GetAdminOverview()
        {
            // Здесь в будущем можно добавить проверку: if (User.Role != "Admin") return Forbid();
            var overview = await _statsService.GetSystemOverviewAsync();
            return Ok(overview);
        }

        [HttpGet("{siteId:guid}/daily")]
        public async Task<IActionResult> GetDailyStats(
            [FromRoute] Guid siteId,
            [FromQuery] DateTime start,
            [FromQuery] DateTime end,
            [FromQuery] double tariff = 4.32) 
        {
            var userId = GetUserIdFromToken();

            var stats = await _statsService.GetDailyStatisticsAsync(siteId, userId, start, end, tariff);

            if (stats == null)
            {
                return NotFound(new { message = "Объект не найден или вы не имеете к нему доступа." });
            }

            return Ok(stats);
        }
        [HttpGet("device/{deviceId:guid}")]
        public async Task<IActionResult> GetDeviceStats(
    [FromRoute] Guid deviceId,
    [FromQuery] DateTime start,
    [FromQuery] DateTime end,
    [FromQuery] double tariff = 4.32)
        {
            var userId = GetUserIdFromToken();

            var stats = await _statsService.GetDeviceStatisticsAsync(deviceId, userId, start, end, tariff);

            if (stats == null)
            {
                return NotFound(new { message = "Устройство не найдено или у вас нет к нему доступа." });
            }

            return Ok(stats);
        }
    }
}