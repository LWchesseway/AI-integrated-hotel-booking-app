using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Entities.Hotel;
using BookingEntity = DoAn.HotelParking.Core.Domain.Entities.Booking.Booking;

namespace DoAn.HotelParking.Core.Domain.Entities.Review;

public class Review
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public int RoomId { get; set; }
    public byte Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public BookingEntity Booking { get; set; } = null!;
    public User Customer { get; set; } = null!;
    public Room Room { get; set; } = null!;
}