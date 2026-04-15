using DoAn.HotelParking.Core.Application.DTOs.Booking;
using DoAn.HotelParking.Core.Application.Interfaces.Booking;
using DoAn.HotelParking.Presentation.Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn.HotelParking.Presentation.Api.Controllers.Booking;

[Route("api/bookings")]
[Authorize]
public class BookingsController : CrudControllerBase<BookingDto, CreateBookingDto, UpdateBookingDto>
{
    public BookingsController(IBookingService service) : base(service)
    {
    }
}