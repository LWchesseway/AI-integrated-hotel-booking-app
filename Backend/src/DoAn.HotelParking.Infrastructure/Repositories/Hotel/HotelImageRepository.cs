using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Hotel;

public class HotelImageRepository : GenericRepository<HotelImage>, IHotelImageRepository
{
    public HotelImageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<HotelImage>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        return await Context.HotelImages
            .Where(i => i.HotelId == hotelId)
            .OrderByDescending(i => i.IsPrimary)
            .ThenBy(i => i.SortOrder)
            .ThenByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task ClearPrimaryAsync(int hotelId, int? exceptImageId = null, CancellationToken cancellationToken = default)
    {
        var query = Context.HotelImages.Where(i => i.HotelId == hotelId && i.IsPrimary);

        if (exceptImageId.HasValue)
        {
            query = query.Where(i => i.Id != exceptImageId.Value);
        }

        var items = await query.ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.IsPrimary = false;
        }
    }
}
