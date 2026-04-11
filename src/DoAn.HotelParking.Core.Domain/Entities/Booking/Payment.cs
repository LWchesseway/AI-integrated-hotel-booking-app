using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Domain.Entities.Booking;

public class Payment
{
    public int BookingId { get; set; }
    public decimal Amount { get; set; }
    public string? Method { get; set; }
    public PaymentStatus Status { get; set; }
    public string? TransactionCode { get; set; }
    public DateTime CreatedAt { get; set; }

    public Booking Booking { get; set; } = null!;
}