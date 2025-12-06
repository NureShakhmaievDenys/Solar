// Presentation.API/Controllers/SitesController.cs
using Application.DTOs.Devices;
using Application.DTOs.Sites; // 👈 Убедитесь, что этот using есть
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
    public class SitesController : ControllerBase
    {
        private readonly SiteService _siteService;

        public SitesController(SiteService siteService)
        {
            _siteService = siteService;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdString!);
        }


        // GET /api/sites
        [HttpGet]
        public async Task<IActionResult> GetUserSites()
        {
            var userId = GetUserIdFromToken();
            var sites = await _siteService.GetSitesForUserAsync(userId);
            return Ok(sites);
        }
        [HttpPost]
        public async Task<IActionResult> CreateSite([FromBody] CreateSiteRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserIdFromToken();
            var newSite = await _siteService.CreateSiteAsync(request, userId);
            return CreatedAtAction(nameof(GetUserSites), new { id = newSite.Id }, newSite);
        }

        // PUT /api/sites/{siteId}
        [HttpPut("{siteId:guid}")]
        public async Task<IActionResult> UpdateSite([FromRoute] Guid siteId, [FromBody] UpdateSiteRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserIdFromToken();
            var updatedSite = await _siteService.UpdateSiteAsync(siteId, request, userId);

            if (updatedSite == null)
            {
                return NotFound(new { message = "Объект не найден или он не принадлежит вам." });
            }
            return Ok(updatedSite);
        }
        // DELETE /api/sites/{siteId}
        [HttpDelete("{siteId:guid}")]
        public async Task<IActionResult> DeleteSite([FromRoute] Guid siteId)
        {
            var userId = GetUserIdFromToken();
            var success = await _siteService.DeleteSiteAsync(siteId, userId);

            if (!success)
            {
                return NotFound(new { message = "Объект не найден или он не принадлежит вам." });
            }
            return NoContent(); 
        }


        // POST /api/sites/{siteId}/devices
        [HttpPost("{siteId:guid}/devices")]
        public async Task<IActionResult> AddDeviceToSite([FromRoute] Guid siteId, [FromBody] CreateDeviceRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserIdFromToken();
            var newDevice = await _siteService.AddDeviceToSiteAsync(request, siteId, userId);
            if (newDevice == null)
            {
                return NotFound(new { message = "Объект не найден или он не принадлежит вам." });
            }
            return StatusCode(201, newDevice);
        }

        // PUT /api/sites/{siteId}/devices/{deviceId}
        [HttpPut("{siteId:guid}/devices/{deviceId:guid}")]
        public async Task<IActionResult> UpdateDevice([FromRoute] Guid siteId, [FromRoute] Guid deviceId, [FromBody] UpdateDeviceRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = GetUserIdFromToken();
            var updatedDevice = await _siteService.UpdateDeviceAsync(siteId, deviceId, request, userId);

            if (updatedDevice == null)
            {
                return NotFound(new { message = "Устройство (или объект) не найдено или не принадлежит вам." });
            }
            return Ok(updatedDevice);
        }

        // DELETE /api/sites/{siteId}/devices/{deviceId}
        [HttpDelete("{siteId:guid}/devices/{deviceId:guid}")]
        public async Task<IActionResult> DeleteDevice([FromRoute] Guid siteId, [FromRoute] Guid deviceId)
        {
            var userId = GetUserIdFromToken();
            var success = await _siteService.DeleteDeviceAsync(siteId, deviceId, userId);

            if (!success)
            {
                return NotFound(new { message = "Устройство (или объект) не найдено или не принадлежит вам." });
            }
            return NoContent(); 
        }
    }
}