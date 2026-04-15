using DoAn.HotelParking.Core.Domain.Enums;
using BookingEntity = DoAn.HotelParking.Core.Domain.Entities.Booking.Booking;
using ReviewEntity = DoAn.HotelParking.Core.Domain.Entities.Review.Review;

namespace DoAn.HotelParking.Core.Domain.Entities.Hotel;

public class Room
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public int RoomTypeId { get; set; }
    public string? RoomNumber { get; set; }
    public int Capacity { get; set; }
    public decimal Price { get; set; }
    public RoomStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    public Hotel Hotel { get; set; } = null!;
    public RoomType RoomType { get; set; } = null!;
    public ICollection<RoomImage> RoomImages { get; set; } = new HashSet<RoomImage>();
    public ICollection<BookingEntity> Bookings { get; set; } = new HashSet<BookingEntity>();
    public ICollection<ReviewEntity> Reviews { get; set; } = new HashSet<ReviewEntity>();
}