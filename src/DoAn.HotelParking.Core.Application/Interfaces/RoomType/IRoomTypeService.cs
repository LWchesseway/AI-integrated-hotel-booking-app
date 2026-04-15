using DoAn.HotelParking.Core.Application.DTOs.RoomType;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.RoomType;

public interface IRoomTypeService : ICrudService<RoomTypeDto, CreateRoomTypeDto, UpdateRoomTypeDto>
{
}