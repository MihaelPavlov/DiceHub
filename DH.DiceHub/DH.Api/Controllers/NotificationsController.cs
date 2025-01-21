using DH.Adapter.Authentication.Filters;
using DH.Domain.Adapters.Authentication.Enums;
using DH.Domain.Adapters.PushNotifications;
using DH.Domain.Adapters.PushNotifications.Messages.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DH.Api.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationsController : ControllerBase
{
    readonly IPushNotificationsService pushNotificationsService;

    public NotificationsController(IPushNotificationsService pushNotificationsService)
    {
        this.pushNotificationsService = pushNotificationsService;
    }

    [HttpGet]
    [ActionAuthorize(UserAction.NotificationCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<GetUserNotificationsModel>))]
    public async Task<IActionResult> GetNotificationList(int id, CancellationToken cancellationToken)
    {
        var result = await this.pushNotificationsService.GetNotificationsByUserId(cancellationToken);
        return Ok(result);
    }

    [HttpGet("are-any-active")]
    [ActionAuthorize(UserAction.NotificationCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    public async Task<IActionResult> AreAnyActiveNotifcations(CancellationToken cancellationToken)
    {
        var result = await this.pushNotificationsService.AreAnyActiveNotifcations(cancellationToken);
        return Ok(result);
    }

    [HttpPost("marked-as-viewed")]
    [ActionAuthorize(UserAction.NotificationCRUD)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkedNotiicationAsViewed([FromBody] int id, CancellationToken cancellationToken)
    {
        await this.pushNotificationsService.MarkedNotificationAsViewed(id, cancellationToken);
        return Ok();
    }
}
