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

    [HttpGet("modules")]
    [HasPermission("permission.read")]
    public async Task<IActionResult> GetGroupedByModule(CancellationToken cancellationToken = default)
    {
        var modules = await _permissionManagementService.GetGroupedByModuleAsync(cancellationToken);
        return Ok(ApiResponse<IEnumerable<PermissionModuleDto>>.Ok(modules));
    }

    [HttpGet("modules/{module}")]
    [HasPermission("permission.read")]
    public async Task<IActionResult> GetByModule(string module, CancellationToken cancellationToken = default)
    {
        var permissions = await _permissionManagementService.GetByModuleAsync(module, cancellationToken);
        return Ok(ApiResponse<IEnumerable<PermissionDto>>.Ok(permissions));
    }

    [HttpPost]
    [HasPermission("permission.manage")]
    public async Task<IActionResult> Create([FromBody] CreatePermissionDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _permissionManagementService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<PermissionDto>.Ok(created, "Created", 201));
    }

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
