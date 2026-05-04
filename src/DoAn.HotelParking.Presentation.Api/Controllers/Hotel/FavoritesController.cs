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
[HasPermission("favorite.manage")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteHotelService _favoriteHotelService;

    public FavoritesController(IFavoriteHotelService favoriteHotelService)
    {
        _favoriteHotelService = favoriteHotelService;
    }

    [HttpGet("my-favorites")]
    public async Task<IActionResult> GetMyFavorites(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var favorites = await _favoriteHotelService.GetUserFavoritesAsync(userId, cancellationToken);
        return Ok(ApiResponse<IEnumerable<HotelDto>>.Ok(favorites, "Favorites retrieved"));
    }

    [HttpGet("{hotelId:int}/is-favorite")]
    public async Task<IActionResult> IsFavorite(int hotelId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        var isFavorite = await _favoriteHotelService.IsFavoriteAsync(userId, hotelId, cancellationToken);
        return Ok(ApiResponse<FavoriteStatusDto>.Ok(new FavoriteStatusDto { IsFavorite = isFavorite }, "Favorite status retrieved"));
    }

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
