using System.ComponentModel.DataAnnotations;
namespace DoAn.HotelParking.Core.Application.DTOs.Rooms;

public class CreateRoomTypeDto
{
    [Required(ErrorMessage = "Mã khách sạn là bắt buộc")]
    public string HotelId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên loại phòng là bắt buộc")]
    public string RoomTypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
}