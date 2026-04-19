using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class CancelBookingRequestDto
{
    [MaxLength(255)]
    public string? Reason { get; set; }
}
