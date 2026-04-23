using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;

namespace DoAn.HotelParking.Core.Application.Services.Hotel;

public class FavoriteHotelService : IFavoriteHotelService
{
    private readonly IFavoriteHotelRepository _favoriteHotelRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FavoriteHotelService(
        IFavoriteHotelRepository favoriteHotelRepository,
        IHotelRepository hotelRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _favoriteHotelRepository = favoriteHotelRepository;
        _hotelRepository = hotelRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HotelDto>> GetUserFavoritesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var hotels = await _favoriteHotelRepository.GetUserFavoriteHotelsAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<HotelDto>>(hotels);
    }

    public Task<bool> IsFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default)
    {
        return _favoriteHotelRepository.IsFavoriteAsync(userId, hotelId, cancellationToken);
    }

    public async Task<ToggleFavoriteResponseDto> ToggleFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await _hotelRepository.GetByIdAsync(hotelId, cancellationToken);
        if (hotel is null)
        {
            throw new KeyNotFoundException("Hotel not found.");
        }

        var isFavorite = await _favoriteHotelRepository.IsFavoriteAsync(userId, hotelId, cancellationToken);

        if (isFavorite)
        {
            await _favoriteHotelRepository.RemoveFavoriteAsync(userId, hotelId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ToggleFavoriteResponseDto
            {
                IsFavorite = false,
                Message = "Removed from favorites"
            };
        }

        await _favoriteHotelRepository.AddFavoriteAsync(userId, hotelId, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ToggleFavoriteResponseDto
        {
            IsFavorite = true,
            Message = "Added to favorites"
        };
    }
}
