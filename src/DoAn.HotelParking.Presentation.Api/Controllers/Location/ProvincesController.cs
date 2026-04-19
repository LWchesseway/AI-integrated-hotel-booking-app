using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Location;
using DoAn.HotelParking.Core.Application.Interfaces.Location;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Location;

[Route("api/provinces")]
[Authorize(Roles = "Admin")]
[ApiController]
public class ProvincesController : ControllerBase
{
    private readonly IProvinceService _provinceService;

    public ProvincesController(IProvinceService service)
    {
        _provinceService = service;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _provinceService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<ProvinceDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _provinceService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<ProvinceDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<ProvinceDto>.Ok(item));
    }

    [HttpPost]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Create([FromBody] CreateProvinceDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _provinceService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<ProvinceDto>.Ok(created, "Created", 201));
    }

    [HttpPut("{id:int}")]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProvinceDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _provinceService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<ProvinceDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<ProvinceDto>.Ok(updated, "Updated"));
    }

    [HttpDelete("{id:int}")]
    [HasPermission("location.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _provinceService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}
