using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.RoomType;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.RoomType;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.RoomType;

public class RoomTypeService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Hotel.RoomType, RoomTypeDto, CreateRoomTypeDto, UpdateRoomTypeDto>, IRoomTypeService
{
    public RoomTypeService(
        IRoomTypeRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}