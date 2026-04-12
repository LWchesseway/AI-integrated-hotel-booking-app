using DoAn.HotelParking.Core.Application.Interfaces.Rooms;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả phòng
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _roomService.GetAllRoomsAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy phòng theo phân trang
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageIndex <= 0 || pageSize <= 0)
            return BadRequest(new { success = false, message = "PageIndex và PageSize phải lớn hơn 0" });

        var result = await _roomService.GetRoomsPagedAsync(pageIndex, pageSize, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy chi tiết phòng theo Id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _roomService.GetRoomByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Lấy danh sách phòng theo khách sạn
    /// </summary>
    [HttpGet("hotel/{hotelId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByHotel(int hotelId, CancellationToken cancellationToken)
    {
        var result = await _roomService.GetRoomsByHotelAsync(hotelId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy danh sách phòng trống trong khoảng thời gian
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailable([FromQuery] int hotelId, [FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, CancellationToken cancellationToken)
    {
        if (checkOut <= checkIn)
            return BadRequest(new { success = false, message = "CheckOut phải sau CheckIn" });

        var result = await _roomService.GetAvailableRoomsAsync(hotelId, checkIn, checkOut, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Tạo phòng mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRoomDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _roomService.CreateRoomAsync(dto, cancellationToken);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    /// <summary>
    /// Cập nhật thông tin phòng
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _roomService.UpdateRoomAsync(id, dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Xóa phòng
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await _roomService.DeleteRoomAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Thay đổi trạng thái phòng
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] int status, CancellationToken cancellationToken)
    {
        var result = await _roomService.ChangeRoomStatusAsync(id, status, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
