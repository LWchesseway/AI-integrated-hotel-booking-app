using DoAn.HotelParking.Core.Domain.Enums;

namespace DoAn.HotelParking.Core.Domain.Entities.Hotel;

public class Hotel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Street { get; set; }
    public string? Ward { get; set; }
    public string? Province { get; set; }
    public string? Phone { get; set; }
    public string? Description { get; set; }
    public HotelStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Room> Rooms { get; set; } = new HashSet<Room>();
}