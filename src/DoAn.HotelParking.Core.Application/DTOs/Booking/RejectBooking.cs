using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class RejectBookingDto
{
    [MaxLength(256, ErrorMessage = "Lý do từ chối không được vượt quá 256 ký tự")]
    public string Reason { get; set; } = string.Empty;
}