using System.ComponentModel.DataAnnotations;

namespace DoAn.HotelParking.Core.Application.DTOs.Hotel;

public class UpdateHotelImageDto
{
    [Required]
    [MaxLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ObjectKey { get; set; }

    public bool IsPrimary { get; set; }

    public int SortOrder { get; set; }
}
