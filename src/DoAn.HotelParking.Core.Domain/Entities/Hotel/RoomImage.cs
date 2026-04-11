namespace DoAn.HotelParking.Core.Domain.Entities.Hotel;

public class RoomImage
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsMain { get; set; }

    public Room Room { get; set; } = null!;
}