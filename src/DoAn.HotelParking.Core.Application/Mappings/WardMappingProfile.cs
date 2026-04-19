using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Location;
using DoAn.HotelParking.Core.Domain.Entities.Location;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class WardMappingProfile : Profile
{
    public WardMappingProfile()
    {
        CreateMap<Ward, WardDto>();
        CreateMap<CreateWardDto, Ward>();
        CreateMap<UpdateWardDto, Ward>();
    }
}
