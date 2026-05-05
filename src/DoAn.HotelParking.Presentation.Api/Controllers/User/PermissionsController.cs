using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.User;

[ApiController]
[Route("api/permissions")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionManagementService _permissionManagementService;

    public PermissionsController(IPermissionManagementService permissionManagementService)
    {
        _permissionManagementService = permissionManagementService;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach quyen theo phan trang va bo loc.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="keyword">Dau vao: Tu khoa tim kiem (query).</param>
    /// <param name="module">Dau vao: Ten module can loc (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach quyen.</returns>
    [HttpGet]
    [HasPermission("permission.read")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? keyword = null,
        [FromQuery] string? module = null,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _permissionManagementService.GetPagedAsync(
            pageIndex,
            pageSize,
            keyword,
            module,
            cancellationToken);

        return Ok(ApiPagedResponse<PermissionDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin quyen theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id quyen.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua quyen neu tim thay.</returns>
    [HttpGet("{id:int}")]
    [HasPermission("permission.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var permission = await _permissionManagementService.GetByIdAsync(id, cancellationToken);
        if (permission is null)
        {
            return NotFound(ApiResponse<PermissionDto>.Fail("Permission not found", 404));
        }

        return Ok(ApiResponse<PermissionDto>.Ok(permission));
    }

    /// <summary>
    /// Chuc nang: Lay danh sach quyen duoc nhom theo module.
    /// </summary>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach module va quyen.</returns>
    [HttpGet("modules")]
    [HasPermission("permission.read")]
    public async Task<IActionResult> GetGroupedByModule(CancellationToken cancellationToken = default)
    {
        var modules = await _permissionManagementService.GetGroupedByModuleAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<PermissionModuleDto>>.Ok(modules));
    }

    /// <summary>
    /// Chuc nang: Lay danh sach quyen theo module.
    /// </summary>
    /// <param name="module">Dau vao: Ten module.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach quyen theo module.</returns>
    [HttpGet("modules/{module}")]
    [HasPermission("permission.read")]
    public async Task<IActionResult> GetByModule(string module, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionManagementService.GetByModuleAsync(module, cancellationToken);
        return Ok(ApiResponse<IEnumerable<PermissionDto>>.Ok(permissions));
    }

    /// <summary>
    /// Chuc nang: Tao moi quyen.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao quyen.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua quyen vua tao.</returns>
    [HttpPost]
    [HasPermission("permission.manage")]
    public async Task<IActionResult> Create([FromBody] CreatePermissionDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _permissionManagementService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<PermissionDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat quyen theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id quyen.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat quyen.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua quyen sau cap nhat.</returns>
    [HttpPut("{id:int}")]
    [HasPermission("permission.manage")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdatePermissionDto dto,
        CancellationToken cancellationToken = default)
    {
        var updated = await _permissionManagementService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<PermissionDto>.Fail("Permission not found", 404));
        }

        return Ok(ApiResponse<PermissionDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa quyen theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id quyen.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [HasPermission("permission.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _permissionManagementService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Permission not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}
