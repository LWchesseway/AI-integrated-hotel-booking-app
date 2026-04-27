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

        CreateMap<Room, RoomDetailDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status))
            .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel != null ? src.Hotel.Name : null))
            .ForMember(dest => dest.RoomTypeName, opt => opt.MapFrom(src => src.RoomType != null ? src.RoomType.Name : null));

        CreateMap<CreateRoomDto, Room>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (RoomStatus)src.Status));

        CreateMap<UpdateRoomDto, Room>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (RoomStatus)src.Status));
    }
}