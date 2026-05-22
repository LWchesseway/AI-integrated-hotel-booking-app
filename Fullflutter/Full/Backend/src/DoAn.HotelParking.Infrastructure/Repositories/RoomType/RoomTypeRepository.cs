using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.RoomType;

public class RoomTypeRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.RoomType>, IRoomTypeRepository
{
    public RoomTypeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<DoAn.HotelParking.Core.Domain.Entities.Hotel.RoomType>> GetByHotelIdWithRoomsAsync(
        int hotelId,
        CancellationToken cancellationToken = default)
    {
        return await Context.RoomTypes
            .AsNoTracking()
            .Where(rt => rt.Rooms.Any(r => r.HotelId == hotelId))
            .Include(rt => rt.Rooms.Where(r => r.HotelId == hotelId))
                .ThenInclude(r => r.Hotel)
            .ToListAsync(cancellationToken);
    }
}