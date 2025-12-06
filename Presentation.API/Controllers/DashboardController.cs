
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace Presentation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdString!);
        }

        [HttpGet("{siteId:guid}/realtime")]
        public async Task<IActionResult> GetRealtimeDashboardData(Guid siteId)
        {
            var userId = GetUserIdFromToken();
            var dashboardData = await _dashboardService.GetRealtimeDataAsync(siteId, userId);

            if (dashboardData == null)
            {
                return NotFound(new { message = "Об'єкт не знайдено або він не належить вам." });
            }

            return Ok(dashboardData);
        }
    }
}