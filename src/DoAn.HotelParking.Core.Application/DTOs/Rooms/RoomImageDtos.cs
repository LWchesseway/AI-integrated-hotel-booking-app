namespace DoAn.HotelParking.Core.Application.DTOs.Rooms;

public class RoomImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int RoomId { get; set; }
}
public class CreateRoomImageDto
{
    public string Url { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public bool IsMain { get; set; }
}
public class UpdateRoomImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public bool IsMain { get; set; }
}
public class RoomImageResponseDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public bool IsMain { get; set; }
}