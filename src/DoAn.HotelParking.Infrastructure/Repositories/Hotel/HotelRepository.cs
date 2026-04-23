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