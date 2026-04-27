using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        CreateMap<Hotel, HotelDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status));

        CreateMap<Hotel, HotelDetailDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (byte)src.Status))
            .ForMember(dest => dest.WardName, opt => opt.MapFrom(src => src.Ward != null ? src.Ward.Name : null))
            .ForMember(dest => dest.ProvinceName, opt => opt.MapFrom(src => src.Ward != null && src.Ward.Province != null ? src.Ward.Province.Name : null));

        CreateMap<CreateHotelDto, Hotel>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (HotelStatus)src.Status));

        CreateMap<UpdateHotelDto, Hotel>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (HotelStatus)src.Status));
    }
}