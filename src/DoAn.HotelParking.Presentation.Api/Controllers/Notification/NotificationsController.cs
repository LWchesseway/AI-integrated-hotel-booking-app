using DoAn.HotelParking.Core.Application.DTOs.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Notification;

[Route("api/notifications")]
[Authorize]
public class NotificationsController : CrudControllerBase<NotificationDto, CreateNotificationDto, UpdateNotificationDto>
{
    public NotificationsController(INotificationService service) : base(service)
    {
    }
}