using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.SystemConfig;
using SystemConfigEntity = DoAn.HotelParking.Core.Domain.Entities.System.SystemConfig;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class SystemConfigMappingProfile : Profile
{
    public SystemConfigMappingProfile()
    {
        CreateMap<SystemConfigEntity, SystemConfigDto>();
    }
}
