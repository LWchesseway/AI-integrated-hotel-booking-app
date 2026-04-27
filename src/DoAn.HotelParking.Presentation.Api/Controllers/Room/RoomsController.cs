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

    [HttpPost]
    [HasPermission("room.manage")]
    public async Task<IActionResult> Create([FromBody] CreateRoomDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _roomService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<RoomDto>.Ok(created, "Created", 201));
    }

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