using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Location;
using DoAn.HotelParking.Core.Domain.Entities.Location;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class ProvinceMappingProfile : Profile
{
    public ProvinceMappingProfile()
    {
        CreateMap<Province, ProvinceDto>();
        CreateMap<CreateProvinceDto, Province>();
        CreateMap<UpdateProvinceDto, Province>();
    }
}
