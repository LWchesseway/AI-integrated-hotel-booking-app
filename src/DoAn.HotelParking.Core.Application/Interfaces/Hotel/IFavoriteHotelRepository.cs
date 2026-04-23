using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IFavoriteHotelRepository
{
    Task<FavoriteHotel?> GetByUserAndHotelAsync(int userId, int hotelId, CancellationToken cancellationToken = default);
    Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> GetUserFavoriteHotelsAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> IsFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default);
    Task<FavoriteHotel> AddFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default);
    Task<bool> RemoveFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default);
}
