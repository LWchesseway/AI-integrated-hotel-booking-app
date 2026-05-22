using DoAn.HotelParking.Core.Application.DTOs.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IFavoriteHotelService
{
    Task<IEnumerable<HotelDto>> GetUserFavoritesAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> IsFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default);
    Task<ToggleFavoriteResponseDto> ToggleFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default);
}
