using DoAn.HotelParking.Core.Domain.Entities.Auth;
using DoAn.HotelParking.Core.Domain.Entities.Location;
using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Domain.Entities.Hotel;

public class Hotel
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int WardId { get; set; }
    public string? Name { get; set; }
    public string? Street { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public HotelStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public User Owner { get; set; } = null!;
    public Ward Ward { get; set; } = null!;
    public ICollection<Room> Rooms { get; set; } = new HashSet<Room>();
    public ICollection<HotelImage> HotelImages { get; set; } = new HashSet<HotelImage>();
    public ICollection<FavoriteHotel> FavoriteHotels { get; set; } = new HashSet<FavoriteHotel>();
}