namespace DoAn.HotelParking.Core.Domain.Entities.Hotel;

public class TimeSlot
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Room Room { get; set; } = null!;
}
