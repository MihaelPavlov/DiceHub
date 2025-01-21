using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class MessageController : ControllerBase
{
    readonly IPushNotificationsService pushNotificationsService;

    public MessageController(IPushNotificationsService pushNotificationsService)
    {
        this.pushNotificationsService = pushNotificationsService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMessageAsync([FromBody] MessageRequest request)
    {
        await this.pushNotificationsService.SendUserNotificationAsync(request);
        return Ok("Push notification sent successfully!");
    }
}
