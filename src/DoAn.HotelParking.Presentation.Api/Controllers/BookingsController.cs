using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsController> _logger;

    public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả đặt phòng
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetAllBookingsAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy đặt phòng theo phân trang
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageIndex <= 0 || pageSize <= 0)
            return BadRequest(new { success = false, message = "PageIndex và PageSize phải lớn hơn 0" });

        var result = await _bookingService.GetBookingsPagedAsync(pageIndex, pageSize, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy chi tiết đặt phòng
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetBookingByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Lấy danh sách đặt phòng của khách hàng
    /// </summary>
    [HttpGet("customer/{customerId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyBookings(int customerId, CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetMyBookingsAsync(customerId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy danh sách đặt phòng chờ duyệt
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken)
    {
        var result = await _bookingService.GetPendingBookingsAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Tạo đặt phòng mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBookingDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _bookingService.CreateBookingAsync(dto, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    /// <summary>
    /// Duyệt đặt phòng
    /// </summary>
    [HttpPost("{id}/approve")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Approve(int id, [FromQuery] int approvedBy, CancellationToken cancellationToken)
    {
        var result = await _bookingService.ApproveBookingAsync(id, approvedBy, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Hủy đặt phòng
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(int id, [FromQuery] int cancelledBy, CancellationToken cancellationToken)
    {
        var result = await _bookingService.CancelBookingAsync(id, cancelledBy, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Tải lên ảnh chứng minh thanh toán
    /// </summary>
    [HttpPost("{id}/payment-proof")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadPaymentProof(int id, [FromBody] UploadPaymentProofDto dto, CancellationToken cancellationToken)
    {
        var result = await _bookingService.UploadPaymentProofAsync(id, dto.ProofUrl, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Xác nhận thanh toán
    /// </summary>
    [HttpPost("{id}/confirm-payment")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmPayment(int id, CancellationToken cancellationToken)
    {
        var result = await _bookingService.ConfirmPaymentAsync(id, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public class UploadPaymentProofDto
{
    public string ProofUrl { get; set; } = null!;
}
