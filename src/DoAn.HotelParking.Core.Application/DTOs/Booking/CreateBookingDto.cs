using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class CreateBookingDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public int CustomerId { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal TotalAmount { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal DepositAmount { get; set; }

    public string? PaymentProofUrl { get; set; }
    public string? Note { get; set; }
    public byte Status { get; set; } = 0;
}