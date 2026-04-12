namespace DoAn.HotelParking.Core.Application.DTOs.Rooms;

public class RoomTypeDto
{
    public int Id { get; set; }
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IEnumerable<RoomDto> Rooms { get; set; } = new List<RoomDto>();
}