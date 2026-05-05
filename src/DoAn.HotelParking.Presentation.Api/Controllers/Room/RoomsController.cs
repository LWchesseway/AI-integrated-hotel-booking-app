using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Room;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Room;

[Route("api/rooms")]
[Authorize(Roles = "Admin,Owner")]
[ApiController]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService service)
    {
        _roomService = service;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach phong theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach phong.</returns>
    [HttpGet]
    [HasPermission("room.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _roomService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<RoomDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay danh sach phong theo hotelId.
    /// </summary>
    /// <param name="hotelId">Dau vao: Id khach san (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach phong.</returns>
    [HttpGet("by-hotel")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByHotelId([FromQuery] int hotelId, CancellationToken cancellationToken = default)
    {
        if (hotelId <= 0)
        {
            return BadRequest(ApiResponse<IEnumerable<RoomDetailDto>>.Fail("hotelId must be greater than 0.", 400));
        }

        var items = await _roomService.GetByHotelIdAsync(hotelId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<RoomDetailDto>>.Ok(items));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin phong theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id phong.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua phong neu tim thay.</returns>
    [HttpGet("{id:int}")]
    [HasPermission("room.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _roomService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<RoomDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<RoomDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Lay danh sach phong theo roomTypeId.
    /// </summary>
    /// <param name="roomTypeId">Dau vao: Id loai phong (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach phong.</returns>
    [HttpGet("by-room-type")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByRoomTypeId([FromQuery] int roomTypeId, CancellationToken cancellationToken = default)
    {
        if (roomTypeId <= 0)
        {
            return BadRequest(ApiResponse<IEnumerable<RoomDetailDto>>.Fail("roomTypeId must be greater than 0.", 400));
        }

        var items = await _roomService.GetByRoomTypeIdAsync(roomTypeId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<RoomDetailDto>>.Ok(items));
    }

    /// <summary>
    /// Chuc nang: Tao moi phong.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao phong.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua phong vua tao.</returns>
    [HttpPost]
    [HasPermission("room.manage")]
    public async Task<IActionResult> Create([FromBody] CreateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _roomService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<RoomDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat phong theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id phong.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat phong.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua phong sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    [HasPermission("room.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _roomService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<RoomDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<RoomDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa phong theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id phong.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [HasPermission("room.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _roomService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}