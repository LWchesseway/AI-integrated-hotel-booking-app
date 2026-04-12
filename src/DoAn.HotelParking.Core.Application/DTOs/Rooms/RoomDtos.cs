namespace DoAn.HotelParking.Core.Application.DTOs.Rooms;

using DoAn.HotelParking.Core.Domain.Enums;
public class RoomDto
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Capacity { get; set; } = null!;
    public decimal Price { get; set; }
    public RoomStatus Status { get; set; }
    public bool IsDeleted { get; set; }

    public int AvailableRooms { get; set; }
    public double AverageRating { get; set; }
    public string RoomTypeName { get; set; } = null!;
    public int HotelId { get; set; }
    public string HotelName { get; set; } = null!;
    public IEnumerable<RoomImageResponseDto> Images { get; set; } = new List<RoomImageResponseDto>();
}