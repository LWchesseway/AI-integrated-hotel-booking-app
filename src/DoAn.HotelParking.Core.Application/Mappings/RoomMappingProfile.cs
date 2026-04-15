using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Room;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class RoomMappingProfile : Profile
{
    public RoomMappingProfile()
    {
        CreateMap<Room, RoomDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status));

        CreateMap<CreateRoomDto, Room>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (RoomStatus)src.Status));

        CreateMap<UpdateRoomDto, Room>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (RoomStatus)src.Status));
    }
}