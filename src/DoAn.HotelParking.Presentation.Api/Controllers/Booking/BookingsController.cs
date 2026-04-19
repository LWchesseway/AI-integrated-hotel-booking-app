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

    [HttpPost]
    [Authorize(Roles = "Admin,Owner")]
    [HasPermission("booking.manage")]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _bookingService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<BookingDto>.Ok(created, "Created", 201));
    }

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
        var rawUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(rawUserId, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user from token.");
        }

        return userId;
    }
}