using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class HotelImageMappingProfile : Profile
{
    public HotelImageMappingProfile()
    {
        CreateMap<HotelImage, HotelImageDto>();
        CreateMap<CreateHotelImageDto, HotelImage>();
        CreateMap<UpdateHotelImageDto, HotelImage>();
    }
}
