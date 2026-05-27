using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Room;

public class UpdateRoomDto
{
    [Required]
    public int RoomTypeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string RoomNumber { get; set; } = string.Empty;

    [Range(1, 100)]
    public int Capacity { get; set; }

    [Range(typeof(decimal), "0", "999999999")]
    public decimal Price { get; set; }

    public byte Status { get; set; } = 1;

    public bool IsDeleted { get; set; }
}