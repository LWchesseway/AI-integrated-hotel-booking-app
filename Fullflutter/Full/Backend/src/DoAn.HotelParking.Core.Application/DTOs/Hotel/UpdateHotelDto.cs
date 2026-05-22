using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Hotel;

public class UpdateHotelDto
{
    public int? OwnerId { get; set; }

    [Required]
    public int WardId { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Street { get; set; }

    [MaxLength(15)]
    public string? Phone { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public byte Status { get; set; } = 1;

    public bool IsDeleted { get; set; }
}