using ECommerce_NET.Data;
using ECommerce_NET.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce_NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotification _notificationService;

        public NotificationController(INotification notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _ = int.TryParse(userIdClaim, out int id);

            var notifications = await _notificationService.GetUserNotifications(id);

            return Ok(notifications);
        }
    }
}
