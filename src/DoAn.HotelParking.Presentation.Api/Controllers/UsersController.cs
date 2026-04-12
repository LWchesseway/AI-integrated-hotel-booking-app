using DoAn.HotelParking.Core.Application.Interfaces.User;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả người dùng
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllUsersAsync(cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy người dùng theo phân trang
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        if (pageIndex <= 0 || pageSize <= 0)
            return BadRequest(new { success = false, message = "PageIndex và PageSize phải lớn hơn 0" });

        var result = await _userService.GetUsersPagedAsync(pageIndex, pageSize, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Lấy chi tiết người dùng
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserByIdAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Lấy người dùng theo email
    /// </summary>
    [HttpGet("by-email/{email}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
    {
        var result = await _userService.GetUserByEmailAsync(email, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Tìm kiếm người dùng
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string searchTerm, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest(new { success = false, message = "SearchTerm không thể trống" });

        var result = await _userService.SearchUsersAsync(searchTerm, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Cập nhật thông tin người dùng
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _userService.UpdateUserAsync(id, dto, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Cập nhật avatar người dùng
    /// </summary>
    [HttpPut("{id}/avatar")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateAvatar(int id, [FromBody] UpdateAvatarDto dto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(dto.AvatarUrl))
            return BadRequest(new { success = false, message = "AvatarUrl không thể trống" });

        var result = await _userService.UpdateUserAvatarAsync(id, dto.AvatarUrl, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Thay đổi trạng thái người dùng
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] int status, CancellationToken cancellationToken)
    {
        var result = await _userService.ChangeUserStatusAsync(id, status, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Xóa người dùng (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, [FromQuery] int deletedBy, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUserAsync(id, deletedBy, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Khôi phục người dùng
    /// </summary>
    [HttpPost("{id}/restore")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Restore(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.RestoreUserAsync(id, cancellationToken);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Gán vai trò cho người dùng
    /// </summary>
    [HttpPost("{id}/roles/{roleId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRole(int id, int roleId, CancellationToken cancellationToken)
    {
        var result = await _userService.AssignRoleAsync(id, roleId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Hủy vai trò của người dùng
    /// </summary>
    [HttpDelete("{id}/roles/{roleId}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveRole(int id, int roleId, CancellationToken cancellationToken)
    {
        var result = await _userService.RemoveRoleAsync(id, roleId, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}

public class UpdateAvatarDto
{
    public string AvatarUrl { get; set; } = null!;
}
