using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Hotel;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Hotel;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.Hotel;

public class HotelService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Hotel.Hotel, HotelDto, CreateHotelDto, UpdateHotelDto>, IHotelService
{
    public HotelService(
        IHotelRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}