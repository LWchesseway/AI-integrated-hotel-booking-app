using AutoMapper;
using DoAn.HotelParking.Core.Application.DTOs.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Base;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Core.Application.Services.Base;

namespace DoAn.HotelParking.Core.Application.Services.Booking;

public class BookingService : CrudService<DoAn.HotelParking.Core.Domain.Entities.Booking.Booking, BookingDto, CreateBookingDto, UpdateBookingDto>, IBookingService
{
    public BookingService(
        IBookingRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
        : base(repository, unitOfWork, mapper)
    {
    }
}