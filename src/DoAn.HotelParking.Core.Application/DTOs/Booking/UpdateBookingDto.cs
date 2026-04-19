using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class UpdateBookingDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public int TimeSlotId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    [Range(1, 20)]
    public int GuestCount { get; set; } = 1;

    [Range(typeof(decimal), "0", "999999999")]
    public decimal PaidAmount { get; set; }

    public string? Note { get; set; }
    public byte Status { get; set; }
    public int? CancelledBy { get; set; }
    public string? CancelReason { get; set; }
    public DateTime? CancelledAt { get; set; }
}