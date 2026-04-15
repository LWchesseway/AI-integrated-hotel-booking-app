using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Base;

[ApiController]

[Authorize]
public abstract class CrudControllerBase<TDto, TCreateDto, TUpdateDto> : ControllerBase
{
    protected readonly ICrudService<TDto, TCreateDto, TUpdateDto> Service;

    protected CrudControllerBase(ICrudService<TDto, TCreateDto, TUpdateDto> service)
    {
        Service = service;
    }

    [HttpGet]
    public virtual async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await Service.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<TDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    [HttpGet("{id:int}")]
    public virtual async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await Service.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<TDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<TDto>.Ok(item));
    }

    [HttpPost]
    public virtual async Task<IActionResult> Create([FromBody] TCreateDto dto, CancellationToken cancellationToken = default)
    {
        var created = await Service.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<TDto>.Ok(created, "Created", 201));
    }

    [HttpPut("{id:int}")]
    public virtual async Task<IActionResult> Update(int id, [FromBody] TUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await Service.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<TDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<TDto>.Ok(updated, "Updated"));
    }

    [HttpDelete("{id:int}")]
    public virtual async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await Service.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}