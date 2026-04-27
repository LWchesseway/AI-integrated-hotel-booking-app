using DoAn.HotelParking.Core.Application.DTOs.Room;

namespace DoAn.HotelParking.Core.Application.DTOs.RoomType;

public class RoomTypeDetailDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<RoomDetailDto> Rooms { get; set; } = new();
}
