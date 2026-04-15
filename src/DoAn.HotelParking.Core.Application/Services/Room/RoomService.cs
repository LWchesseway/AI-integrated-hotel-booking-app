using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Room;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Room;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.Room;

public class RoomService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Hotel.Room, RoomDto, CreateRoomDto, UpdateRoomDto>, IRoomService
{
    public RoomService(
        IRoomRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}