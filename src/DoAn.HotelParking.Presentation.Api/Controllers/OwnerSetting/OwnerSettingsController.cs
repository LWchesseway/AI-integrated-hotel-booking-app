using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.OwnerSetting;
using DoAn.HotelParking.Core.Application.Interfaces.OwnerSetting;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.OwnerSetting;

[ApiController]
[Route("api/owner/settings")]
[Authorize(Roles = "Owner")]
public class OwnerSettingsController : ControllerBase
{
    private readonly IOwnerSettingService _ownerSettingService;

    public OwnerSettingsController(IOwnerSettingService ownerSettingService)
    {
        _ownerSettingService = ownerSettingService;
    }

    /// <summary>
    /// Chuc nang: Lay thong tin cau hinh cua owner dang dang nhap.
    /// </summary>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua cau hinh owner.</returns>
    [HttpGet]
    [HasPermission("ownersetting.read")]
    public async Task<IActionResult> GetSettings(CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var result = await _ownerSettingService.GetSettingsWithDefaultsAsync(ownerId, cancellationToken);
        return Ok(ApiResponse<OwnerSettingResponseDto>.Ok(result, "Owner settings retrieved"));
    }

    /// <summary>
    /// Chuc nang: Cap nhat cau hinh owner dang dang nhap.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu cap nhat cau hinh owner.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua cap nhat.</returns>
    [HttpPut]
    [HasPermission("ownersetting.manage")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateOwnerSettingDto dto, CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        await _ownerSettingService.UpdateSettingsAsync(ownerId, dto, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "Owner settings updated"));
    }

    /// <summary>
    /// Chuc nang: Cap nhat thong tin ngan hang cho owner dang dang nhap.
    /// </summary>
    /// <param name="dto">Dau vao: Du lieu cap nhat thong tin ngan hang.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult thong bao ket qua cap nhat.</returns>
    [HttpPut("bank-info")]
    [HttpPost("bank-info")]
    [HasPermission("ownersetting.manage")]
    public async Task<IActionResult> UpdateBankInfo([FromForm] UpdateBankInfoDto dto, CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        await _ownerSettingService.UpdateBankInfoAsync(ownerId, dto, cancellationToken);
        return Ok(ApiResponse<object>.Ok(null, "Bank information updated"));
    }

    /// <summary>
    /// Chuc nang: Kiem tra tinh day du cua thong tin ngan hang.
    /// </summary>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua ket qua kiem tra.</returns>
    [HttpGet("bank-info/validate")]
    [HasPermission("ownersetting.read")]
    public async Task<IActionResult> ValidateBankInfo(CancellationToken cancellationToken = default)
    {
        var ownerId = GetCurrentUserId();
        var isValid = await _ownerSettingService.ValidateBankInfoAsync(ownerId, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(isValid, isValid ? "Bank information is complete" : "Bank information is incomplete"));
    }

    private int GetCurrentUserId()
    {
        var rawUserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (!int.TryParse(rawUserId, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user from token.");
        }

        return userId;
    }
}
