using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace DoAn.HotelParking.Infrastructure.Repositories.Room;

public class RoomRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<DoAn.HotelParking.Core.Domain.Entities.Hotel.Room?> GetByIdWithHotelAsync(int id, CancellationToken cancellationToken = default)
    {
        return Context.Rooms
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }
}