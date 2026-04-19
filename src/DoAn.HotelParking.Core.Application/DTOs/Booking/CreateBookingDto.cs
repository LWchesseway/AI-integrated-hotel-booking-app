using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class CreateBookingDto
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

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }

    [MaxLength(100)]
    public string? TransactionCode { get; set; }

    [MaxLength(255)]
    public string? PaymentNote { get; set; }

    public string? Note { get; set; }
    public byte Status { get; set; } = 0;
}