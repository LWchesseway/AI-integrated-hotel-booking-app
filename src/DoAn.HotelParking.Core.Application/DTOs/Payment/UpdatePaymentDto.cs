using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Payment;

public class UpdatePaymentDto
{
    [Required]
    public int BookingId { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal Amount { get; set; }

    [MaxLength(50)]
    public string? Method { get; set; }

    public byte Status { get; set; }

    [MaxLength(100)]
    public string? TransactionCode { get; set; }

    [MaxLength(255)]
    public string? Note { get; set; }

    public DateTime? PaidAt { get; set; }
}