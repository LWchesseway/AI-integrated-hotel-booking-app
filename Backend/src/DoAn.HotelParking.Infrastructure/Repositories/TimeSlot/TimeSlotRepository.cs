using DoAn.HotelParking.Core.Application.Interfaces.TimeSlot;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using TimeSlotEntity = DoAn.HotelParking.Core.Domain.Entities.Hotel.TimeSlot;

namespace DoAn.HotelParking.Infrastructure.Repositories.TimeSlot;

public class TimeSlotRepository : GenericRepository<TimeSlotEntity>, ITimeSlotRepository
{
    public TimeSlotRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<TimeSlotEntity?> GetActiveByRoomAndDateRangeAsync(
        int roomId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return Context.TimeSlots
            .AsNoTracking()
            .Where(ts =>
                ts.RoomId == roomId
                && ts.IsActive
                && ts.StartDate <= startDate
                && ts.EndDate >= endDate)
            .OrderBy(ts => ts.StartDate)
            .ThenBy(ts => ts.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeSlotEntity>> GetByRoomIdAsync(int roomId, CancellationToken cancellationToken = default)
    {
        return await Context.TimeSlots
            .AsNoTracking()
            .Where(ts => ts.RoomId == roomId)
            .OrderBy(ts => ts.StartDate)
            .ThenBy(ts => ts.EndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeSlotEntity>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        return await Context.TimeSlots
            .AsNoTracking()
            .Where(ts => ts.Room.HotelId == hotelId)
            .OrderBy(ts => ts.RoomId)
            .ThenBy(ts => ts.StartDate)
            .ThenBy(ts => ts.EndDate)
            .ToListAsync(cancellationToken);
    }
}
