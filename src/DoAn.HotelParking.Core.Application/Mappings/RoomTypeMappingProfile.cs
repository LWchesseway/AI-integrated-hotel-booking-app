using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.RoomType;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class RoomTypeMappingProfile : Profile
{
    public RoomTypeMappingProfile()
    {
        CreateMap<RoomType, RoomTypeDto>();
        CreateMap<RoomType, RoomTypeDetailDto>()
            .AfterMap((src, dest) =>
            {
                foreach (var room in dest.Rooms)
                {
                    room.RoomTypeName ??= src.Name;
                }
            });
        CreateMap<CreateRoomTypeDto, RoomType>();
        CreateMap<UpdateRoomTypeDto, RoomType>();
    }
}