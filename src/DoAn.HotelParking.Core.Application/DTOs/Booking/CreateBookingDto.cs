using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class CreateBookingDto
{

    [Required(ErrorMessage = "RoomId là bắt buộc")]
    public int RoomId { get; set; }

    [Required(ErrorMessage = "CheckInDate là bắt buộc")]
    public DateTime CheckInDate { get; set; }

    [Required(ErrorMessage = "CheckOutDate là bắt buộc")]
    public DateTime CheckOutDate { get; set; }
    public string Note { get; set; } = string.Empty;
}