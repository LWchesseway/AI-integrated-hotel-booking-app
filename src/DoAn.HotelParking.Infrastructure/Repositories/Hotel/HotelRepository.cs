using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Hotel;

public class HotelRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>, IHotelRepository
{
    public HotelRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> SearchByNameWithLocationAsync(
        string hotelName,
        CancellationToken cancellationToken = default)
    {
        var normalizedHotelName = hotelName.Trim().ToLower();

        return await Context.Hotels
            .AsNoTracking()
            .Include(h => h.Ward)
                .ThenInclude(w => w.Province)
            .Where(h => h.Name != null && h.Name.ToLower().Contains(normalizedHotelName))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> GetByProvinceWithLocationAsync(
        string province,
        CancellationToken cancellationToken = default)
    {
        var normalizedProvince = province.Trim().ToLower();

        return await Context.Hotels
            .AsNoTracking()
            .Include(h => h.Ward)
                .ThenInclude(w => w.Province)
            .Where(h => h.Ward.Province.Name.ToLower().Contains(normalizedProvince))
            .ToListAsync(cancellationToken);
    }



    public Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel?> GetHotelWithDetailsForRecommendationAsync(
        int hotelId,
        CancellationToken cancellationToken = default)
    {
        return Context.Hotels
            .AsNoTracking()
            .Include(h => h.Ward)
                .ThenInclude(w => w.Province)
            .Include(h => h.HotelImages)
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Bookings)
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Reviews)
            .FirstOrDefaultAsync(h => h.Id == hotelId && !h.IsDeleted && h.Status == HotelStatus.Active, cancellationToken);
    }

    public Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel?> GetByRoomIdAsync(
        int roomId,
        CancellationToken cancellationToken = default)
    {
        return Context.Hotels
            .AsNoTracking()
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.Rooms.Any(r => r.Id == roomId) && !h.IsDeleted, cancellationToken);
    }
    public async Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel>> GetAllActiveHotelsWithDetailsAsync(
        string? province = null,
        string? ward = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Hotels
            .AsNoTracking()
            .Include(h => h.Ward)
                .ThenInclude(w => w.Province)
            .Include(h => h.HotelImages)
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Bookings)
            .Include(h => h.Rooms)
                .ThenInclude(r => r.Reviews)
            .Where(h => !h.IsDeleted && h.Status == HotelStatus.Active);

        if (!string.IsNullOrWhiteSpace(province))
        {
            var normalizedProvince = province.Trim().ToLower();
            query = query.Where(h => h.Ward.Province.Name.ToLower() == normalizedProvince);
        }

        if (!string.IsNullOrWhiteSpace(ward))
        {
            var normalizedWard = ward.Trim().ToLower();
            query = query.Where(h => h.Ward.Name.ToLower() == normalizedWard);
        }

        return await query.ToListAsync(cancellationToken);
    }
}