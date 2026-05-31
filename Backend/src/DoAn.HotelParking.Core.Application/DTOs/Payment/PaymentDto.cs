namespace DoAn.HotelParking.Core.Application.DTOs.Payment;

public class PaymentDto
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string? Method { get; set; }
    public byte Status { get; set; }
    public string? TransactionCode { get; set; }
    public string? Note { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}