using DoAn.HotelParking.Core.Application.Interfaces.Parking;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ParkingSessionsController : ControllerBase
{
    private readonly IParkingSessionService _parkingSessionService;
    private readonly ILogger<ParkingSessionsController> _logger;

    public ParkingSessionsController(IParkingSessionService parkingSessionService, ILogger<ParkingSessionsController> logger)
    {
        _parkingSessionService = parkingSessionService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả phiên đậu xe
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _parkingSessionService.GetAllSessionsAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy phiên đậu xe theo phân trang
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageIndex <= 0 || pageSize <= 0)
            return BadRequest(new { success = false, message = "PageIndex và PageSize phải lớn hơn 0" });

        var result = await _parkingSessionService.GetSessionsPagedAsync(pageIndex, pageSize, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy chi tiết phiên đậu xe
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _parkingSessionService.GetSessionByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Lấy danh sách phiên đậu xe của người dùng
    /// </summary>
    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUser(int userId, CancellationToken cancellationToken)
    {
        var result = await _parkingSessionService.GetMySessionsAsync(userId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy danh sách phiên đậu xe đang hoạt động
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive(CancellationToken cancellationToken)
    {
        var result = await _parkingSessionService.GetActiveSessions(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Check-in phiên đậu xe
    /// </summary>
    [HttpPost("check-in")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _parkingSessionService.CheckInAsync(dto.UserId, dto.QrUserId, dto.LicensePlate, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Check-out phiên đậu xe
    /// </summary>
    [HttpPost("{id}/check-out")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckOut(int id, [FromBody] CheckOutDto dto, CancellationToken cancellationToken)
    {
        var result = await _parkingSessionService.CheckOutAsync(id, dto.LicensePlate, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Xác nhận phiên đậu xe
    /// </summary>
    [HttpPost("{id}/verify")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Verify(int id, [FromQuery] int verifiedBy, CancellationToken cancellationToken)
    {
        var result = await _parkingSessionService.VerifySessionAsync(id, verifiedBy, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Hủy phiên đậu xe
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var result = await _parkingSessionService.CancelSessionAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }
}

public class CheckInDto
{
    public int UserId { get; set; }
    public int QrUserId { get; set; }
    public string LicensePlate { get; set; } = null!;
}

public class CheckOutDto
{
    public string LicensePlate { get; set; } = null!;
}
