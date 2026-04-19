using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Location;
using DoAn.HotelParking.Core.Application.Interfaces.Location;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Location;

[Route("api/wards")]
[Authorize(Roles = "Admin")]
[ApiController]
public class WardsController : ControllerBase
{
    private readonly IWardService _wardService;

    public WardsController(IWardService service)
    {
        _wardService = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _wardService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<WardDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _wardService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<WardDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<WardDto>.Ok(item));
    }

    [HttpPost]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Create([FromBody] CreateWardDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _wardService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<WardDto>.Ok(created, "Created", 201));
    }

    [HttpPut("{id:int}")]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWardDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _wardService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<WardDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<WardDto>.Ok(updated, "Updated"));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _wardService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}
