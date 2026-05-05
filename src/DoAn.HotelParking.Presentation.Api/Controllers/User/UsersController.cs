using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.User;
using DoAn.HotelParking.Core.Application.Interfaces.User;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DoAn.HotelParking.Presentation.Api.Controllers.User;

[Route("api/users")]
[Authorize(Roles = "Admin")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService service)
    {
        _userService = service;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach nguoi dung theo phan trang.
    /// </summary>
    /// <param name="pageIndex">Dau vao: Chi so trang (query).</param>
    /// <param name="pageSize">Dau vao: Kich thuoc trang (query).</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach nguoi dung.</returns>
    [HttpGet]
    [HasPermission("user.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _userService.GetPagedAsync(pageIndex, pageSize, cancellationToken);
        return Ok(ApiPagedResponse<UserDto>.Ok(items, pageIndex, pageSize, totalCount));
    }

    /// <summary>
    /// Chuc nang: Cap nhat FCM token cho nguoi dung dang dang nhap.
    /// </summary>
    /// <param name="dto">Dau vao: Thong tin FCM token.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua cap nhat.</returns>
    [HttpPost("fcm-token")]
    public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenDto dto)
    {
        var userIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var result = await _userService.UpdateFcmTokenAsync(userId, dto.Token);

        if (!result)
        {
            return NotFound();
        }

        return Ok(ApiResponse<string>.Ok("FCM token updated"));
    }

    /// <summary>
    /// Chuc nang: Lay thong tin nguoi dung theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id nguoi dung.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua nguoi dung neu tim thay.</returns>
    [HttpGet("{id:int}")]
    [HasPermission("user.read")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var item = await _userService.GetByIdAsync(id, cancellationToken);
        if (item is null)
        {
            return NotFound(ApiResponse<UserDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<UserDto>.Ok(item));
    }

    /// <summary>
    /// Chuc nang: Tao moi nguoi dung.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu tao nguoi dung.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua nguoi dung vua tao.</returns>
    [HttpPost]
    [HasPermission("user.manage")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var created = await _userService.CreateAsync(dto, cancellationToken);
        return Ok(ApiResponse<UserDto>.Ok(created, "Created", 201));
    }

    /// <summary>
    /// Chuc nang: Cap nhat nguoi dung theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id nguoi dung.</param>
    /// <param name="dto">Dau vao: Du lieu cap nhat nguoi dung.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua nguoi dung sau khi cap nhat.</returns>
    [HttpPut("{id:int}")]
    [HasPermission("user.manage")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var updated = await _userService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
        {
            return NotFound(ApiResponse<UserDto>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<UserDto>.Ok(updated, "Updated"));
    }

    /// <summary>
    /// Chuc nang: Xoa nguoi dung theo id.
    /// </summary>
    /// <param name="id">Dau vao: Id nguoi dung.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua xoa.</returns>
    [HttpDelete("{id:int}")]
    [HasPermission("user.manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _userService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail("Not found", 404));
        }

        return Ok(ApiResponse<object>.Ok(null, "Deleted"));
    }
}