using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.Hotel;

public interface IHotelService : ICrudService<HotelDto, CreateHotelDto, UpdateHotelDto>
{
}