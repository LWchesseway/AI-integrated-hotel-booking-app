using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class CustomerCreateBookingRequestDto
{
    [Required]
    public int RoomId { get; set; }

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

    [MaxLength(255)]
    public string? Note { get; set; }
}
