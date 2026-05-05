using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public AdminController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Chuc nang: Admin ep hoan tat don dat phong theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id booking.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua.</returns>
    [HttpPatch("{id:int}/force-complete")]
    [HasPermission("booking.force_complete")]
    public async Task<IActionResult> AdminForceCompleteBooking(int id, CancellationToken cancellationToken = default)
    {
        await _bookingService.AdminForceCompleteBookingAsync(id, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "Booking force-completed successfully"));
    }
}
