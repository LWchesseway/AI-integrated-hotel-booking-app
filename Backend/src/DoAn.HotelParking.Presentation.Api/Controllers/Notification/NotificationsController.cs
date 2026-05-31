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

    /// <summary>
    /// Chuc nang: Lay danh sach thong bao theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach thong bao.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _notificationService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<NotificationDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin thong bao theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id thong bao.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua thong bao neu tim thay.</returns>
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

    /// <summary>
    /// Chuc nang: Tao moi thong bao.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao thong bao.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua thong bao vua tao.</returns>
    [HttpPost]
    [HasPermission("notification.manage")]
    public async Task<IActionResult> Create(
        [FromBody] CreateNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        var created = await _notificationService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<NotificationDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat thong bao theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id thong bao.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat thong bao.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua thong bao sau cap nhat.</returns>
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

    /// <summary>
    /// Chuc nang: Xoa thong bao theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id thong bao.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
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