using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DoAn.HotelParking.Core.Application.DTOs.Base;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Presentation.Api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Hotel;

[ApiController]
[Route("api/favorites")]
[Authorize(Roles = "Customer")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteHotelService _favoriteHotelService;

    public FavoritesController(IFavoriteHotelService favoriteHotelService)
    {
        _favoriteHotelService = favoriteHotelService;
    }

    /// <summary>
    /// Chuc nang: Lay danh sach khach san yeu thich cua nguoi dung.
    /// </summary>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua danh sach khach san yeu thich.</returns>
    [HttpGet("my-favorites")]
    public async Task<IActionResult> GetMyFavorites(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var favorites = await _favoriteHotelService.GetUserFavoritesAsync(userId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<HotelDto>>.Ok(favorites, "Favorites retrieved"));
    }

    /// <summary>
    /// Chuc nang: Kiem tra khach san co nam trong danh sach yeu thich.
    /// </summary>
    /// <param name="hotelId">Dau vao: Id khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua trang thai yeu thich.</returns>
    [HttpGet("{hotelId:int}/is-favorite")]
    public async Task<IActionResult> IsFavorite(int hotelId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var isFavorite = await _favoriteHotelService.IsFavoriteAsync(userId, hotelId, cancellationToken);
        return Ok(ApiResponse<FavoriteStatusDto>.Ok(new FavoriteStatusDto { IsFavorite = isFavorite }, "Favorite status retrieved"));
    }

    /// <summary>
    /// Chuc nang: Them/bo khach san khoi danh sach yeu thich.
    /// </summary>
    /// <param name="hotelId">Dau vao: Id khach san.</param>
    /// <param name="cancellationToken">Dau vao: Token huy yeu cau neu can.</param>
    /// <returns>Dau ra: IActionResult chua ket qua cap nhat yeu thich.</returns>
    [HttpPost("{hotelId:int}/toggle")]
    public async Task<IActionResult> ToggleFavorite(int hotelId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var result = await _favoriteHotelService.ToggleFavoriteAsync(userId, hotelId, cancellationToken);
        return Ok(ApiResponse<ToggleFavoriteResponseDto>.Ok(result, result.Message));
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
