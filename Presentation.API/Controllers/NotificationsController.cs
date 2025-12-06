
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
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private Guid GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdString!);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var userId = GetUserIdFromToken();
            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            return Ok(notifications);
        }

        [HttpPost("{notificationId:guid}/read")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
        {
            var userId = GetUserIdFromToken();
            var success = await _notificationService.MarkAsReadAsync(notificationId, userId);
            if (!success)
            {
                return NotFound(new { message = "Оповещение не найдено или уже прочитано." });
            }
            return NoContent();
        }

        [HttpDelete("{notificationId:guid}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            var userId = GetUserIdFromToken();
            var success = await _notificationService.DeleteNotificationAsync(notificationId, userId);

            if (!success)
            {
                return NotFound(new { message = "Оповещение не найдено или не принадлежит вам." });
            }
            return NoContent(); 
        }
    }
}