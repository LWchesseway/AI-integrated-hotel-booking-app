using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Role;
using DoAn.HotelParking.Core.Application.Interfaces.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Role;

[Route("api/roles")]
[Authorize(Roles = "Admin")]
[ApiController]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService service)
    {
        _roleService = service;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach vai tro theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach vai tro.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _roleService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<RoleDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin vai tro theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id vai tro.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua vai tro neu tim thay.</returns>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _roleService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<RoleDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<RoleDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Tao moi vai tro.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao vai tro.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua vai tro vua tao.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _roleService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<RoleDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat vai tro theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id vai tro.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat vai tro.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua vai tro sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _roleService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<RoleDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<RoleDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa vai tro theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id vai tro.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _roleService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}