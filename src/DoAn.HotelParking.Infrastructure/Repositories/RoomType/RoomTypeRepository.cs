using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using DoAn.HotelParking.Infrastructure.Data;
using DoAn.HotelParking.Infrastructure.Repositories.Base;

namespace DoAn.HotelParking.Infrastructure.Repositories.RoomType;

public class RoomTypeRepository : GenericRepository<DoAn.HotelParking.Core.Domain.Entities.Hotel.RoomType>, IRoomTypeRepository
{
    public RoomTypeRepository(ApplicationDbContext context) : base(context)
    {
    }
}