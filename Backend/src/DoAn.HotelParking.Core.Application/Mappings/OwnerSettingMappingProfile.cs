using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.OwnerSetting;
using OwnerSettingEntity = DoAn.HotelParking.Core.Domain.Entities.OwnerSetting.OwnerSetting;

namespace DoAn.HotelParking.Core.Application.Mappings;

public class OwnerSettingMappingProfile : Profile
{
    public OwnerSettingMappingProfile()
    {
        CreateMap<OwnerSettingEntity, OwnerSettingDto>();
        CreateMap<CreateOwnerSettingDto, OwnerSettingEntity>();
        CreateMap<UpdateOwnerSettingDto, OwnerSettingEntity>();
    }
}
