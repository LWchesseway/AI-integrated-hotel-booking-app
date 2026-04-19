using DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.TimeSlot;

public class TimeSlotRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>, ITimeSlotRepository
{
    public TimeSlotRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot?> GetDefaultByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        return Context.TimeSlots
            .FirstOrDefaultAsync(ts => ts.HotelId == hotelId && ts.IsDefault && !ts.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        return await Context.TimeSlots
            .Where(ts => ts.HotelId == hotelId && !ts.IsDeleted)
            .OrderByDescending(ts => ts.IsDefault)
            .ThenBy(ts => ts.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task ClearDefaultAsync(int hotelId, int? exceptTimeSlotId = null, CancellationToken cancellationToken = default)
    {
        var query = Context.TimeSlots.Where(ts => ts.HotelId == hotelId && ts.IsDefault);

        if (exceptTimeSlotId.HasValue)
        {
            query = query.Where(ts => ts.Id != exceptTimeSlotId.Value);
        }

        var items = await query.ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            item.IsDefault = false;
        }
    }
}
