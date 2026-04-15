using DoAn.HotelParking.Core.Application.DTOs.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Base;

namespace DoAn.HotelParking.Core.Application.Interfaces.Booking;

public interface IBookingService : ICrudService<BookingDto, CreateBookingDto, UpdateBookingDto>
{
}