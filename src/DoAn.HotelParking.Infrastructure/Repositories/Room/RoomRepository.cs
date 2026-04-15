using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.Room;

public class RoomRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.Room>, IRoomRepository
{
    public RoomRepository(ApplicationDbContext context) : base(context)
    {
    }
}