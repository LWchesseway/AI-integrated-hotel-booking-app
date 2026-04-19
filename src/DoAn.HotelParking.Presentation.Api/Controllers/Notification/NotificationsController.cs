using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Notification;
using DoAn.HotelParking.Core.Application.Interfaces.Notification;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Notification;

[Route("api/notifications")]
[Authorize]
[HasPermission("notification.read")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService service)
    {
        _notificationService = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _notificationService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<NotificationDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _notificationService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<NotificationDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<NotificationDto>.Ok(item));
    }

    [HttpPost]
    [HasPermission("notification.manage")]
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        var created = await _notificationService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<NotificationDto>.Ok(created, "Created", 201));
    }

    [HttpPut("{id:int}")]
    [HasPermission("notification.manage")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        var updated = await _notificationService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<NotificationDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<NotificationDto>.Ok(updated, "Updated"));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("notification.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _notificationService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}