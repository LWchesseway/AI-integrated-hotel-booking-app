using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.RoomType;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class RoomTypeMappingProfile : Profile
{
    public RoomTypeMappingProfile()
    {
        CreateMap<RoomType, RoomTypeDto>();
        CreateMap<CreateRoomTypeDto, RoomType>();
        CreateMap<UpdateRoomTypeDto, RoomType>();
    }
}