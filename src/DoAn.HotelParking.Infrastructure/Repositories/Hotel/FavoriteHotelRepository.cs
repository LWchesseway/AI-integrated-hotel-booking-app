using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Hotel;

public class FavoriteHotelRepository : GenericRepository<FavoriteHotel>, IFavoriteHotelRepository
{
    public FavoriteHotelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<FavoriteHotel?> GetByUserAndHotelAsync(int userId, int hotelId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(fh => fh.UserId == userId && fh.HotelId == hotelId, cancellationToken);
    }

    public async Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> GetUserFavoriteHotelsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.FavoriteHotels
            .AsNoTracking()
            .Where(fh => fh.UserId == userId)
            .Include(fh => fh.Hotel)
                .ThenInclude(h => h.HotelImages)
            .Select(fh => fh.Hotel)
            .Where(h => !h.IsDeleted && h.Status == HotelStatus.Active)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(fh => fh.UserId == userId && fh.HotelId == hotelId, cancellationToken);
    }

    public async Task<FavoriteHotel> AddFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default)
    {
        var favorite = new FavoriteHotel
        {
            UserId = userId,
            HotelId = hotelId,
            CreatedAt = DateTime.UtcNow
        };

        await AddAsync(favorite, cancellationToken);
        return favorite;
    }

    public async Task<bool> RemoveFavoriteAsync(int userId, int hotelId, CancellationToken cancellationToken = default)
    {
        var favorite = await GetByUserAndHotelAsync(userId, hotelId, cancellationToken);
        if (favorite is null)
        {
            return false;
        }

        Remove(favorite);
        return true;
    }
}
