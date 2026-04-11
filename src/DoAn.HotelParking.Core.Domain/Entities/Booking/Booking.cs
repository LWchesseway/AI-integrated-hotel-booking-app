using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using DoAn.HotelParking.Core.Domain.Enums;
using ReviewEntity = DoAn.HotelParking.Core.Domain.Entities.Review.Review;

namespace DoAn.HotelParking.Core.Domain.Entities.Booking;

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DepositAmount { get; set; }
    public string? PaymentProofUrl { get; set; }
    public string? Note { get; set; }
    public BookingStatus Status { get; set; }
    public int? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Room Room { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public User? ApprovedByUser { get; set; }
    public User? CancelledByUser { get; set; }
    public Payment? Payment { get; set; }
    public ICollection<ReviewEntity> Reviews { get; set; } = new HashSet<ReviewEntity>();
}