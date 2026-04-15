namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public string? PaymentProofUrl { get; set; }
    public string? Note { get; set; }
    public byte Status { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}