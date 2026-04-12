using System.ComponentModel.DataAnnotations;
namespace DoAn.HotelParking.Core.Application.DTOs.Rooms;

public class CreateRoomDtos
{
    [Required(ErrorMessage = "Mã RoomType là bắt buộc")]
    public string RoomTypeId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số phòng là bắt buộc")]
    public int RoomNumber { get; set; }
    [Required(ErrorMessage = "Sức chứa là bắt buộc")]
    public string? Capacity { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương")]
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}