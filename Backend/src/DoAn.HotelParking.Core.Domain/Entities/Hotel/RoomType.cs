namespace DoAn.HotelParking.Core.Domain.Entities.Hotel;

public class RoomType
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Room> Rooms { get; set; } = new HashSet<Room>();
}