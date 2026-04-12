namespace DoAn.HotelParking.Core.Application.DTOs.Booking;

public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string UserPhone { get; set; } = string.Empty;
    public int StaffId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public string Note { get; set; } = string.Empty;
    public string PaymentProofUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? ApprovedBy { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? CancelledBy { get; set; }
    public string? CancelledByName { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
}