using System.ComponentModel.DataAnnotations;
namespace DoAn.HotelParking.Core.Application.DTOs.Rooms;

public class UpdateRoomDto
{
    [Required(ErrorMessage = "Số phòng là bắt buộc")]
    public int RoomNumber { get; set; }

    [Required(ErrorMessage = "Mã loại phòng là bắt buộc")]
    public int RoomTypeId { get; set; }
    [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số dương")]
    public decimal Price { get; set; }
    [Required(ErrorMessage = "Sức chứa là bắt buộc")]
    public string? Capacity { get; set; }

    public string? Description { get; set; }
}