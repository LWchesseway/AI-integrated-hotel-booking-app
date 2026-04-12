using System.ComponentModel.DataAnnotations;
using DoAn.HotelParking.Core.Domain.Enums;
namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class UpdateBookingStatusDto
{
    [Required(ErrorMessage = "Trạng thái mới là bắt buộc")]
    public BookingStatus NewStatus { get; set; }
}