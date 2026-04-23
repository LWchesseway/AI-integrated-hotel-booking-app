using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.TimeSlot;
using DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.TimeSlot;

[Route("api/time-slots")]
[Authorize(Roles = "Admin,Owner")]
[ApiController]
public class TimeSlotsController : ControllerBase
{
    private readonly ITimeSlotService _timeSlotService;

    public TimeSlotsController(ITimeSlotService service)
    {
        _timeSlotService = service;
    }

    [HttpGet]
    [HasPermission("timeslot.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _timeSlotService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<TimeSlotDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    [HasPermission("timeslot.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _timeSlotService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<TimeSlotDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<TimeSlotDto>.Ok(item));
    }

    [HttpPost]
    [HasPermission("timeslot.manage")]
    public async Task<IActionResult> Create([FromBody] CreateTimeSlotDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _timeSlotService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<TimeSlotDto>.Ok(created, "Created", 201));
    }

    [HttpPut("{id:int}")]
    [HasPermission("timeslot.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTimeSlotDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _timeSlotService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<TimeSlotDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<TimeSlotDto>.Ok(updated, "Updated"));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("timeslot.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _timeSlotService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }

    [HttpGet("hotel/{hotelId:int}")]
    [HasPermission("timeslot.read")]
    public async Task<IActionResult> GetByHotelId(int hotelId, CancellationToken cancellationToken = default)
    {
        var items = await _timeSlotService.GetByHotelIdAsync(hotelId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<TimeSlotDto>>.Ok(items));
    }

    [HttpGet("room/{roomId:int}")]
    [HasPermission("timeslot.read")]
    public async Task<IActionResult> GetByRoomId(int roomId, CancellationToken cancellationToken = default)
    {
        var items = await _timeSlotService.GetByRoomIdAsync(roomId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<TimeSlotDto>>.Ok(items));
    }
}
