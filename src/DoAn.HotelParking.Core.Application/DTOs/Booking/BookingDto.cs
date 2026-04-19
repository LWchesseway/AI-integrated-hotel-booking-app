namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int TimeSlotId { get; set; }
    public int CustomerId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NightCount { get; set; }
    public int GuestCount { get; set; }
    public decimal RoomUnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string? Note { get; set; }
    public byte Status { get; set; }
    public int? CancelledBy { get; set; }
    public string? CancelReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}