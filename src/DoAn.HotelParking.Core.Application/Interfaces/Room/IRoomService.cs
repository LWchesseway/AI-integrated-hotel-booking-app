using DoAn.HotelParking.Core.Application.DTOs.Room;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.Room;

public interface IRoomService : ICrudService<RoomDto, CreateRoomDto, UpdateRoomDto>
{
}