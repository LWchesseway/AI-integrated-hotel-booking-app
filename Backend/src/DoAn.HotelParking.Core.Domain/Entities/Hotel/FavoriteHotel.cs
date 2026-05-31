using DoAn.HotelParking.Core.Domain.Entities.Auth;

namespace DoAn.HotelParking.Core.Domain.Entities.Hotel;

public class FavoriteHotel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int HotelId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
    public Hotel Hotel { get; set; } = null!;
}
