using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.RoomType;
using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.RoomType;

[Route("api/room-types")]
[Authorize(Roles = "Admin,Owner")]
[ApiController]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;

    public RoomTypesController(IRoomTypeService service)
    {
        _roomTypeService = service;
    }

    [HttpGet]
    [HasPermission("room.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _roomTypeService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<RoomTypeDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    [HasPermission("room.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _roomTypeService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<RoomTypeDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<RoomTypeDto>.Ok(item));
    }

    [HttpGet("by-hotel")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByHotelId([FromQuery] int hotelId, CancellationToken cancellationToken = default)
    {
        if (hotelId <= 0)
        {
            return BadRequest(ApiResponse<IEnumerable<RoomTypeDetailDto>>.Fail("hotelId must be greater than 0.", 400));
        }

        var items = await _roomTypeService.GetByHotelIdAsync(hotelId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<RoomTypeDetailDto>>.Ok(items));
    }

    [HttpPost]
    [HasPermission("room.manage")]
    public async Task<IActionResult> Create([FromBody] CreateRoomTypeDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _roomTypeService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<RoomTypeDto>.Ok(created, "Created", 201));
    }

    [HttpPut("{id:int}")]
    [HasPermission("room.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomTypeDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _roomTypeService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<RoomTypeDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<RoomTypeDto>.Ok(updated, "Updated"));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("room.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _roomTypeService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}