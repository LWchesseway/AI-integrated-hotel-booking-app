using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DoAn.HotelParking.Core.Application.DTOs.Booking;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Booking;

[Route("api/bookings")]
[Authorize]
[ApiController]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService service)
    {
        _bookingService = service;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach booking theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach booking.</returns>
    [HttpGet]
    [Authorize(Roles = "Admin,Owner")]
    [HasPermission("booking.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _bookingService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<BookingDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin booking theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id booking.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua booking neu tim thay.</returns>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Owner")]
    [HasPermission("booking.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _bookingService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<BookingDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<BookingDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Tao moi booking (admin/owner).
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao booking.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua booking vua tao.</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Owner")]
    [HasPermission("booking.manage")]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _bookingService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat booking theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id booking.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat booking.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua booking sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Owner")]
    [HasPermission("booking.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _bookingService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<BookingDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<BookingDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa booking theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id booking.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Owner")]
    [HasPermission("booking.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _bookingService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }

    /// <summary>
    /// Chuc nang: Tao yeu cau booking cho khach hang dang dang nhap.
    /// </summary>
    /// <param name="request">Dau vao: Thong tin yeu cau booking.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua booking vua tao.</returns>
    [HttpPost("request")]
    [Authorize(Roles = "Customer")]
    [HasPermission("booking.manage")]
    public async Task<IActionResult> CreateBookingRequest(
        [FromBody] CustomerCreateBookingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var customerId = GetCurrentUserId();
        var booking = await _bookingService.CreateCustomerBookingAsync(customerId, request, cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(booking, "Booking request created", 201));
    }

    /// <summary>
    /// Chuc nang: Lay danh sach booking cua khach hang dang dang nhap.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach booking cua khach hang.</returns>
    [HttpGet("my-bookings")]
    [Authorize(Roles = "Customer")]
    [HasPermission("booking.read")]
    public async Task<IActionResult> GetMyBookings(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var customerId = GetCurrentUserId();
        var (items, totalCount) = await _bookingService.GetMyBookingsAsync(customerId, pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<BookingDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Huy booking cua khach hang dang dang nhap.
    /// </summary>
    /// <param name="id">Dau vao: Id booking.</param>
    /// <param name="request">Dau vao: Ly do huy booking.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua booking sau khi huy.</returns>
    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = "Customer")]
    [HasPermission("booking.manage")]
    public async Task<IActionResult> CancelMyBooking(
        int id,
        [FromBody] CancelBookingRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var customerId = GetCurrentUserId();
        var booking = await _bookingService.CancelMyBookingAsync(id, customerId, request.Reason, cancellationToken);
        if (booking is null)
        {
            return NotFound(ApiResponse<BookingDto>.Fail("Booking not found or cannot be cancelled", 404));
        }

        return Ok(ApiResponse<BookingDto>.Ok(booking, "Booking cancelled"));
    }

    private int GetCurrentUserId()
    {
        var rawUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(rawUserId, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user from token.");
        }

        return userId;
    }
}