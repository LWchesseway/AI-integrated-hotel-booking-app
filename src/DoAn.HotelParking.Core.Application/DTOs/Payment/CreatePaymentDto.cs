using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Payment;

public class CreatePaymentDto
{
    [Required]
    public int BookingId { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string? Method { get; set; }

    public byte Status { get; set; } = 0;

    [MaxLength(100)]
    public string? TransactionCode { get; set; }
}